#!/bin/bash

echo "🏥 Verificando salud de servicios..."

# Servicios y sus puertos
declare -A services=(
    ["Gateway"]="6001"
    ["Auction"]="7001"
    ["Search"]="7002"
    ["Identity"]="5001"
    ["RabbitMQ Management"]="15672"
    ["PostgreSQL"]="5432"
    ["MongoDB"]="27018"
)

# Función para verificar si un puerto está disponible
check_port() {
    local port=$1
    local service_name=$2
    
    if curl -s --max-time 5 "http://localhost:$port" >/dev/null 2>&1; then
        echo "✅ $service_name (puerto $port): OK"
        return 0
    else
        echo "❌ $service_name (puerto $port): No disponible"
        return 1
    fi
}

# Función para verificar contenedores Docker
check_docker_containers() {
    echo ""
    echo "🐳 Estado de contenedores Docker:"
    
    containers=("auction_service" "search_service" "identity_service" "gateway_service" "auction_db" "search_db" "rabbitmq")
    
    for container in "${containers[@]}"; do
        if docker ps --format "table {{.Names}}" | grep -q "^$container$"; then
            echo "✅ $container: Ejecutándose"
        else
            echo "❌ $container: No ejecutándose"
        fi
    done
}

# Función para verificar logs recientes de errores
check_recent_errors() {
    echo ""
    echo "📋 Verificando logs recientes por errores..."
    
    containers=("auction_service" "search_service" "identity_service" "gateway_service")
    
    for container in "${containers[@]}"; do
        if docker ps --format "table {{.Names}}" | grep -q "^$container$"; then
            error_count=$(docker logs "$container" --since="5m" 2>&1 | grep -i "error\|exception\|fail" | wc -l)
            if [ "$error_count" -gt 0 ]; then
                echo "⚠️  $container: $error_count errores en los últimos 5 minutos"
            else
                echo "✅ $container: Sin errores recientes"
            fi
        fi
    done
}

echo "🔍 Verificando puertos de servicios..."

all_healthy=true

for service in "${!services[@]}"; do
    port=${services[$service]}
    if ! check_port "$port" "$service"; then
        all_healthy=false
    fi
done

# Verificar contenedores Docker
check_docker_containers

# Verificar logs por errores
check_recent_errors

echo ""
if [ "$all_healthy" = true ]; then
    echo "🎉 Todos los servicios están funcionando correctamente!"
    echo ""
    echo "🌐 URLs disponibles:"
    echo "  - API Gateway: http://localhost:6001"
    echo "  - Auction Service: http://localhost:7001"
    echo "  - Search Service: http://localhost:7002"
    echo "  - Identity Service: http://localhost:5001"
    echo "  - RabbitMQ Management: http://localhost:15672 (guest/guest)"
    exit 0
else
    echo "⚠️  Algunos servicios no están disponibles."
    echo ""
    echo "💡 Para diagnosticar:"
    echo "  - docker-compose ps"
    echo "  - docker-compose logs [service-name]"
    echo "  - docker-compose up -d --build"
    exit 1
fi