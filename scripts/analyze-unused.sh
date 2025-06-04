#!/bin/bash

echo "🔍 Analizando referencias y archivos no utilizados..."

# Verificar si ReSharper CLI está disponible
if command -v jb >/dev/null 2>&1; then
    echo "📊 Usando JetBrains ReSharper CLI para análisis..."
    
    # Crear perfil de cleanup temporal
    cat > temp-cleanup.xml << 'EOF'
<Profile name="TempCleanup">
  <AspOptimizeRegisterDirectives>True</AspOptimizeRegisterDirectives>
  <CSUsingOptimization>True</CSUsingOptimization>
  <VBOptimizeImports>True</VBOptimizeImports>
</Profile>
EOF
    
    # Ejecutar análisis de código
    echo "🔍 Ejecutando análisis de código con ReSharper..."
    jb inspectcode "car-shop-course-microservices.sln" --output="temp-analysis.xml" --verbosity=INFO
    
    # Ejecutar cleanup de código
    echo "🧹 Ejecutando cleanup de código..."
    jb cleanupcode "car-shop-course-microservices.sln" --profile="temp-cleanup.xml" --verbosity=INFO
    
    # Limpiar archivos temporales
    rm -f temp-cleanup.xml temp-analysis.xml
    
else
    echo "⚠️  ReSharper CLI no encontrado. Usando análisis básico..."
    
    # Análisis básico con dotnet format
    echo "🔧 Ejecutando dotnet format para optimizar usings..."
    dotnet format --verbosity minimal
    
    # Buscar archivos .cs que podrían no estar siendo utilizados
    echo "📋 Buscando archivos potencialmente no utilizados..."
    
    microservices=("AuctionService" "SearchService" "IdentityService" "GatewayService" "Contracts")
    
    for service in "${microservices[@]}"; do
        service_path="Src/$service"
        if [ -d "$service_path" ]; then
            echo "🔍 Analizando microservicio: $service"
            
            find "$service_path" -name "*.cs" -type f | while read -r file; do
                filename=$(basename "$file" .cs)
                
                # Excluir archivos importantes por convención
                if [[ ! "$filename" =~ (AssemblyInfo|Program|Startup|\.Designer\.|\.Generated\.|Controller|Service|Repository|Migration) ]]; then
                    # Buscar referencias al archivo en el mismo microservicio
                    if ! grep -r --include="*.cs" --exclude="$(basename "$file")" "$filename" "$service_path/" >/dev/null 2>&1; then
                        # Verificar si el archivo se usa en otros microservicios (para Contracts)
                        if [ "$service" = "Contracts" ]; then
                            if ! grep -r --include="*.cs" --exclude="$(basename "$file")" "$filename" Src/ >/dev/null 2>&1; then
                                echo "⚠️  Contrato potencialmente no utilizado: $file"
                            fi
                        else
                            echo "⚠️  Archivo potencialmente no utilizado en $service: $(basename "$file")"
                        fi
                    fi
                fi
            done
        fi
    done
fi

# Verificar referencias de proyectos
echo "📦 Verificando referencias de proyectos..."

find Src -name "*.csproj" -type f | while read -r proj; do
    echo "📂 Analizando: $(basename "$proj")"
    # Aquí puedes agregar lógica adicional para verificar referencias específicas
done

echo "✅ Análisis de archivos no utilizados completado."