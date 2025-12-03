# Guía Rápida - Cómo Autorizar Operaciones en Divisas

## ¡Ya está implementado! 😄

---

## 🎯 CÓMO AUTORIZAR UNA SOLICITUD

### Paso 1: Acceder al Módulo
1. Inicia sesión con un usuario que tenga rol **Ejecutivo**, **Gerente** o **Administrador**
2. En el menú principal, haz clic en **"💱 Autorización de Divisas"**

### Paso 2: Buscar Solicitudes
1. Usa los filtros para encontrar solicitudes:
   - Por rango de fechas
   - Por ID de transacción
   - Por nombre del solicitante
   - Por divisa
   - Por estado (Pendiente, En Revisión, etc.)
2. Haz clic en **"🔍 Buscar"**

### Paso 3: Ver Detalles y Autorizar
1. En la tabla de resultados, haz clic en el botón **"Ver Detalles"** de la solicitud que quieres procesar
2. Se abrirá una ventana con toda la información:
   - ID de transacción
   - Descripción
   - Solicitante
   - Divisa y montos
   - Tasa de cambio
   - Fechas

### Paso 4: Tomar una Decisión

#### Opción A: Marcar En Revisión
- Haz clic en **"📋 Marcar En Revisión"**
- Agrega comentarios (opcional)
- Confirma la acción

#### Opción B: Autorizar ✅
- Haz clic en **"✅ Autorizar"**
- Agrega comentarios de autorización (opcional)
- Confirma la acción
- ¡Listo! La operación queda autorizada

#### Opción C: Rechazar ❌
- Haz clic en **"❌ Rechazar"**
- **IMPORTANTE**: Aparecerá un campo "Motivo de Rechazo" - es OBLIGATORIO llenarlo
- Agrega el motivo del rechazo
- Agrega comentarios adicionales (opcional)
- Confirma la acción

---

## 🔐 PERMISOS REQUERIDOS

### Para Ver Solicitudes:
- Rol: Ejecutivo, Gerente o Administrador

### Para Autorizar:
Depende de la configuración en **"⚙ Config de Roles Divisas"**:
- **Ejecutivo**: Puede autorizar hasta cierto monto (ej: $50,000)
- **Gerente**: Puede autorizar montos mayores (ej: $200,000)
- **Administrador**: Sin límite de monto

---

## 📋 ESTADOS DE UNA SOLICITUD

| Estado | Descripción | Color |
|--------|-------------|-------|
| **Pendiente** | Recién creada, esperando revisión | 🟡 Amarillo |
| **En Revisión** | Alguien la está revisando | 🔵 Azul |
| **Autorizada** | Aprobada y procesada | 🟢 Verde |
| **Rechazada** | No aprobada | 🔴 Rojo |
| **Expirada** | Venció el tiempo de autorización | ⚫ Gris |

---

## ⚠️ IMPORTANTE

### No Puedes Modificar:
- Solicitudes ya **Autorizadas**
- Solicitudes ya **Rechazadas**
- Solicitudes **Expiradas**

### Campos Obligatorios:
- **Motivo de Rechazo**: OBLIGATORIO al rechazar
- **Comentarios**: Opcional pero recomendado

### Auditoría:
- Todas las acciones quedan registradas en el sistema de auditoría
- Se guarda: quién autorizó/rechazó, cuándo y por qué

---

## 🎬 EJEMPLO PRÁCTICO

### Escenario: Autorizar una compra de USD

1. **Usuario**: Juan Pérez (Gerente)
2. **Solicitud**: Compra de $5,000 USD
3. **Monto en MXN**: $85,000 MXN
4. **Tasa**: 17.00

**Pasos**:
1. Juan abre "Autorización de Divisas"
2. Ve la solicitud en estado "Pendiente"
3. Hace clic en "Ver Detalles"
4. Revisa la información:
   - Solicitante: María García
   - Monto: $85,000 MXN → $5,000 USD
   - Tasa: 17.00
5. Agrega comentario: "Aprobado para compra de inventario internacional"
6. Hace clic en "✅ Autorizar"
7. Confirma la acción
8. ¡Listo! La solicitud queda autorizada

---

## 🔧 FUNCIONALIDADES ADICIONALES

### Aplicar Fecha de Expiración
1. Selecciona una o más solicitudes en la tabla
2. Elige una fecha/hora de expiración
3. Haz clic en "Aplicar a Seleccionadas"
4. Las solicitudes expirarán automáticamente si no se procesan a tiempo

### Exportar Reportes
- **📄 PDF**: Exporta a formato PDF
- **📝 Word**: Exporta a formato Word
- **📊 Excel**: Exporta a formato Excel (CSV)

### Configurar Roles
- Haz clic en "⚙ Config de Roles Divisas"
- Configura qué roles pueden autorizar qué montos
- Agrega/elimina configuraciones
- Activa/desactiva configuraciones con el checkbox

---

## ❓ SOLUCIÓN DE PROBLEMAS

### "No veo el botón Ver Detalles"
- Verifica que tu usuario tenga permisos de autorización
- Solo usuarios con rol Ejecutivo, Gerente o Administrador pueden ver este botón

### "No puedo autorizar"
- Verifica que la solicitud esté en estado "Pendiente" o "En Revisión"
- Verifica que tengas permisos para el monto de la solicitud
- Verifica que la solicitud no haya expirado

### "Me pide motivo de rechazo"
- Es obligatorio al rechazar una solicitud
- Proporciona una razón clara y específica

### "Los botones están deshabilitados"
- La solicitud ya fue procesada (Autorizada/Rechazada/Expirada)
- No se pueden modificar solicitudes ya procesadas

---

## 📊 FLUJO COMPLETO

```
1. Usuario solicita operación en divisa
         ↓
2. Se crea solicitud en estado "Pendiente"
         ↓
3. Autorizador revisa → "En Revisión"
         ↓
4. Autorizador decide:
   ├─→ ✅ Autorizar → Estado: "Autorizada"
   ├─→ ❌ Rechazar → Estado: "Rechazada" (con motivo)
   └─→ ⏰ Expira → Estado: "Expirada"
         ↓
5. Queda registrado en auditoría
```

---

## 🎓 TIPS PROFESIONALES

1. **Siempre agrega comentarios**: Ayuda a mantener un historial claro
2. **Revisa bien los montos**: Verifica que la tasa de cambio sea correcta
3. **Motivos claros al rechazar**: Ayuda al solicitante a entender por qué
4. **Usa "En Revisión"**: Si necesitas más tiempo para decidir
5. **Configura expiraciones**: Para solicitudes urgentes o sensibles

---

## ✅ CHECKLIST DE AUTORIZACIÓN

Antes de autorizar, verifica:
- [ ] Identidad del solicitante
- [ ] Monto correcto en MXN y divisa
- [ ] Tasa de cambio actual y razonable
- [ ] Propósito de la operación
- [ ] Tienes permisos para el monto
- [ ] No hay alertas o banderas rojas
- [ ] Agregaste comentarios relevantes

---

## 🚀 ¡LISTO PARA USAR!

El sistema de autorización está completamente funcional. Solo necesitas:
1. Iniciar sesión con un usuario autorizado
2. Ir a "Autorización de Divisas"
3. Seleccionar una solicitud
4. Hacer clic en "Ver Detalles"
5. ¡Autorizar o rechazar!

**Nota**: Si no ves solicitudes, es porque no hay ninguna creada aún. El sistema está listo para cuando se creen solicitudes de operaciones en divisas.

---

**Última actualización**: Diciembre 2025  
**Estado**: ✅ Completamente Funcional  
**Versión**: 1.0
