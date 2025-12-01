# 🏦 Banco Premier - Sistema Bancario Profesional

Sistema bancario completo desarrollado en C# con Windows Forms y PostgreSQL, diseñado con una interfaz elegante y profesional.

## ✨ Características Principales

### 🎨 Diseño Visual Premium
- Paleta de colores corporativa bancaria (azul oscuro, dorado elegante)
- Interfaz moderna con tarjetas (cards) y efectos hover
- Tipografía profesional con Segoe UI
- Diseño responsive y centrado

### 🔐 Seguridad
- Sistema de autenticación de usuarios
- Contraseñas encriptadas
- Validación de sesiones
- Control de acceso por usuario

### 💰 Funcionalidades Bancarias

#### 1. Consulta de Saldo
- Visualización del saldo actual
- Número de cuenta
- Actualización en tiempo real

#### 2. Movimientos
- Depósitos
- Retiros
- Cargos
- Abonos
- Validación de saldo suficiente
- Registro de conceptos

#### 3. Transferencias
- Transferencias entre cuentas
- Validación de cuenta destino en tiempo real
- Confirmación de operación
- Registro bidireccional (origen y destino)

#### 4. Historial de Movimientos
- Vista completa de todas las transacciones
- Ordenamiento por fecha
- Formato de moneda
- Colores alternados para mejor lectura

#### 5. Estado de Cuenta
- Filtrado por rango de fechas
- Resumen del período:
  - Saldo inicial
  - Total de ingresos
  - Total de egresos
  - Saldo final
- Exportación a PDF (en desarrollo)

## 🛠️ Tecnologías Utilizadas

- **Framework**: .NET 8.0 Windows Forms
- **Lenguaje**: C# 12
- **Base de Datos**: PostgreSQL
- **ORM**: Npgsql
- **Arquitectura**: Capas (Presentación, Lógica, Datos)

## 📋 Requisitos Previos

1. .NET 8.0 SDK o superior
2. PostgreSQL 12 o superior
3. Visual Studio 2022 o VS Code
4. Windows 10/11

## 🚀 Instalación

### 1. Clonar el repositorio
```bash
git clone [url-del-repositorio]
cd SistemaBanco
```

### 2. Configurar la Base de Datos

Crear la base de datos en PostgreSQL:

```sql
CREATE DATABASE banco_db;

-- Tabla de usuarios
CREATE TABLE usuarios (
    id_usuario SERIAL PRIMARY KEY,
    usuario VARCHAR(50) UNIQUE NOT NULL,
    contraseña VARCHAR(255) NOT NULL,
    nombre_completo VARCHAR(100) NOT NULL,
    estatus BOOLEAN DEFAULT TRUE,
    fecha_registro TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Tabla de cuentas
CREATE TABLE cuentas (
    id_cuenta SERIAL PRIMARY KEY,
    id_usuario INTEGER REFERENCES usuarios(id_usuario),
    numero_cuenta VARCHAR(20) UNIQUE NOT NULL,
    saldo DECIMAL(15,2) DEFAULT 0.00,
    fecha_apertura TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Tabla de movimientos
CREATE TABLE movimientos (
    id_movimiento SERIAL PRIMARY KEY,
    id_cuenta INTEGER REFERENCES cuentas(id_cuenta),
    tipo VARCHAR(50) NOT NULL,
    monto DECIMAL(15,2) NOT NULL,
    concepto TEXT,
    saldo_anterior DECIMAL(15,2),
    saldo_nuevo DECIMAL(15,2),
    fecha TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Datos de prueba
INSERT INTO usuarios (usuario, contraseña, nombre_completo) 
VALUES ('admin', 'admin123', 'Administrador del Sistema');

INSERT INTO usuarios (usuario, contraseña, nombre_completo) 
VALUES ('jperez', 'pass123', 'Juan Pérez García');

INSERT INTO cuentas (id_usuario, numero_cuenta, saldo) 
VALUES (1, '1001234567', 50000.00);

INSERT INTO cuentas (id_usuario, numero_cuenta, saldo) 
VALUES (2, '1001234568', 25000.00);
```

### 3. Configurar la Cadena de Conexión

Editar el archivo `App.config`:

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <connectionStrings>
    <add name="BancoDB" 
         connectionString="Host=localhost;Port=5432;Database=banco_db;Username=postgres;Password=tu_password" 
         providerName="Npgsql" />
  </connectionStrings>
</configuration>
```

### 4. Compilar y Ejecutar

```bash
dotnet build
dotnet run
```

## 👤 Usuarios de Prueba

| Usuario | Contraseña | Cuenta |
|---------|------------|--------|
| admin | admin123 | 1001234567 |
| jperez | pass123 | 1001234568 |

## 📁 Estructura del Proyecto

```
SistemaBanco/
├── BankTheme.cs              # Tema visual y estilos
├── Database.cs               # Capa de acceso a datos
├── FormLogin.cs              # Pantalla de inicio de sesión
├── FormMenu.cs               # Menú principal
├── FormSaldo.cs              # Consulta de saldo
├── FormMovimiento.cs         # Registro de movimientos
├── FormTransferencia.cs      # Transferencias bancarias
├── FormHistorial.cs          # Historial de movimientos
├── FormEstadoCuenta.cs       # Estado de cuenta
├── Program.cs                # Punto de entrada
├── App.config                # Configuración
└── SistemaBanco.csproj       # Archivo de proyecto
```

## 🎨 Paleta de Colores

- **Azul Corporativo**: #003366
- **Azul Secundario**: #0066CC
- **Dorado Elegante**: #D4AF37
- **Gris Claro**: #F5F5F5
- **Verde Éxito**: #28A745
- **Rojo Peligro**: #DC3545

## 🔄 Próximas Mejoras

- [ ] Exportación de estados de cuenta a PDF
- [ ] Gráficos de gastos e ingresos
- [ ] Notificaciones de movimientos
- [ ] Límites de retiro diario
- [ ] Autenticación de dos factores
- [ ] Recuperación de contraseña
- [ ] Historial de sesiones
- [ ] Reportes analíticos
- [ ] Soporte multi-moneda
- [ ] API REST para integración

## 📝 Notas de Desarrollo

- Las advertencias de nullable son normales en Windows Forms
- El sistema usa transacciones implícitas de PostgreSQL
- Los movimientos se registran con saldo anterior y nuevo para auditoría
- Las transferencias crean dos movimientos (origen y destino)

## 🤝 Contribuciones

Las contribuciones son bienvenidas. Por favor:

1. Fork el proyecto
2. Crea una rama para tu feature (`git checkout -b feature/AmazingFeature`)
3. Commit tus cambios (`git commit -m 'Add some AmazingFeature'`)
4. Push a la rama (`git push origin feature/AmazingFeature`)
5. Abre un Pull Request

## 📄 Licencia

Este proyecto es de código abierto y está disponible bajo la licencia MIT.

## 👨‍💻 Autor

Desarrollado con ❤️ para demostrar capacidades de desarrollo bancario profesional.

---

**Banco Premier** - *Banca Digital Segura* 🏦
