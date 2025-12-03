# 🔧 Configurar Conexión a Base de Datos

## ❌ Error Actual
**"Host desconocido"** - No puede conectarse a la base de datos

## ✅ Solución

### Paso 1: Obtener tus credenciales de Supabase

1. Abre tu proyecto en **Supabase**
2. Ve a **Settings** (Configuración) → **Database**
3. Busca la sección **Connection String** o **Connection Info**
4. Copia los siguientes datos:
   - **Host**: `db.ovfaxfhvcjrvujtgiaaf.supabase.co`
   - **Database**: `postgres`
   - **User**: `postgres`
   - **Password**: ModuloBanco2025
   - **Port**: `5432`

    postgresql://postgres:ModuloBanco2025@db.ovfaxfhvcjrvujtgiaaf.supabase.co:5432/postgres

### Paso 2: Actualizar App.config

Abre el archivo `App.config` y reemplaza la línea de connectionString con tus datos:

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <connectionStrings>
    <add name="BancoDB" 
         connectionString="Host=TU_HOST_AQUI;Port=5432;Database=postgres;Username=postgres;Password=TU_PASSWORD_AQUI;SSL Mode=Require;Trust Server Certificate=true;" 
         providerName="Npgsql" />
  </connectionStrings>
</configuration>
```


**Ejemplo:**
```xml
<add name="BancoDB" 
     connectionString="Host=db.abcdefghijklmnop.supabase.co;Port=5432;Database=postgres;Username=postgres;Password=MiPassword123;SSL Mode=Require;Trust Server Certificate=true;" 
     providerName="Npgsql" />
```

### Paso 3: Verificar la conexión

Después de actualizar `App.config`:

1. **Guarda el archivo**
2. **Compila**: `dotnet build`
3. **Ejecuta**: `dotnet run`
4. **Intenta iniciar sesión**

### Paso 4: Probar conexión (Opcional)

Si quieres probar la conexión antes de iniciar sesión:

1. Abre `Program.cs`
2. Descomenta estas líneas:
   ```csharp
   TestConexion.ProbarConexion();
   return;
   ```
3. Ejecuta: `dotnet run`
4. Verás si la conexión funciona

## 🔍 Cómo encontrar tu Host de Supabase

### Opción 1: Desde el Dashboard
1. Ve a tu proyecto en Supabase
2. Settings → Database
3. Copia el **Host** (algo como: `db.xxxxx.supabase.co`)

### Opción 2: Desde Connection String
Supabase te da una cadena completa como:
```
postgresql://postgres:[YOUR-PASSWORD]@db.xxxxx.supabase.co:5432/postgres
```

De ahí extraes:
- **Host**: `db.xxxxx.supabase.co`
- **Password**: Lo que pusiste al crear el proyecto

## ⚠️ Importante

- **NO compartas** tu contraseña de base de datos
- **NO subas** el archivo `App.config` con credenciales reales a GitHub
- Si usas Git, agrega `App.config` al `.gitignore`

## 🆘 Si aún no funciona

1. Verifica que tu proyecto de Supabase esté activo
2. Verifica que la contraseña sea correcta
3. Intenta conectarte desde Supabase SQL Editor para confirmar que la BD funciona
4. Verifica que tu firewall no esté bloqueando la conexión

## 📝 Credenciales Actuales (INCORRECTAS)

```
Host: db.ovfaxfhvcjrvujtgiaaf.supabase.co
Password: ModuloBanco2025
```

Estas credenciales parecen ser de ejemplo o de un proyecto antiguo.
**Necesitas usar TUS credenciales reales de Supabase.**
