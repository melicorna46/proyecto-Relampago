# Proyecto Relámpago — Sistema de Gestión de Proyectos SCRUM (MVP)

Aplicación web para gestionar un Sprint SCRUM de extremo a extremo: roles y equipo,
Product Backlog priorizado, Sprint con tablero, Daily/impedimentos, Definition of
Done, Review, Retrospective e indicador de avance (Burndown).

##  Enlaces

- **Repositorio:** https://github.com/melicorna46/proyecto-Relampago
- **Aplicación publicada:** https://tiusr29pl.cuc-carrera-ti.ac.cr/ScrumMvp/

##  Arquitectura

- **Patrón:** ASP.NET MVC clásico (Model-View-Controller)
- **Capas:**
  - `Models/` — entidades del dominio (Usuario, Proyecto, Equipo, Historia, Sprint, Tarea, Daily, Impedimento, DoD, Review, Retrospectiva, Burndown)
  - `Data/` — acceso a datos (consultas MySQL vía MySql.Data.MySqlClient)
  - `Controllers/` — lógica de aplicación y enrutamiento por rol
  - `Views/` — vistas Razor (.cshtml) organizadas por módulo
  - `Filters/` — control de acceso/autorización por rol
  - `Helpers/` — utilidades compartidas

##  Tecnologías

| Componente        | Tecnología                                  |
|--------------------|---------------------------------------------|
| Backend            | ASP.NET MVC, .NET Framework 4.8             |
| Base de datos       | MySQL (MySql.Data.MySqlClient)              |
| Frontend            | Razor Views (.cshtml), Bootstrap, Chart.js (Burndown) |
| Hosting             | Plesk / IIS                                  |
| Control de versiones | Git / GitHub                               |

##  Configuración

1. Clonar el repositorio y abrir `ScrumMvp.sln` en Visual Studio.
2. Restaurar el paquete NuGet `MySql.Data` si no se restaura automáticamente.
3. Crear la base de datos MySQL y ejecutar el script `scrum_mvp.sql` (incluido en el repo).
4. Configurar el connection string en `Web.config`:
   ```xml
   <connectionStrings>
     <add name="ScrumDb"
          connectionString="Server=TU_SERVIDOR;Port=3306;Database=TU_BASE;Uid=TU_USUARIO;Pwd=TU_PASSWORD;"
          providerName="MySql.Data.MySqlClient" />
   </connectionStrings>
   ```
5. Compilar en modo **Release** y publicar (Folder) o correr localmente con IIS Express.

### Despliegue en Plesk

1. Crear un directorio virtual con **"Crear aplicación"** marcado.
2. Confirmar que el sitio tenga asignado **.NET Framework 4.8** (Herramientas de desarrollo → Configuración ASP.NET).
3. Subir el contenido publicado (`bin/`, `Content/`, `Scripts/`, `Views/`, `Web.config`, `Global.asax`, `favicon.ico`) al directorio virtual.
4. Reiniciar el Application Pool.

##  Credenciales / datos de prueba

| Rol            | Usuario (email) | Contraseña |
|-----------------|------------------|------------|
| Product Owner   | jph1@cuc.ac.cr   | admin123   |
| Scrum Master    | jph1@cuc.ac.cr   | admin123   |
| Developer       | jph2@cuc.ac.cr   | admin123   |

##  Backlog implementado (36 HU del MVP)

### Persona 1 — Fundación, equipo y Product Backlog (13 HU)
| HU | Descripción | Estado |
|----|--------------|--------|
| HU-001 | Registro de usuario | ✅ Implementada |
| HU-002 | Inicio de sesión | ✅ Implementada |
| HU-009 | Asignar responsabilidades (PO, SM, Developers) | ✅ Implementada |
| HU-007 | Crear equipo Scrum | ✅ Implementada |
| HU-013 | Crear producto/proyecto | ✅ Implementada |
| HU-015 | Definir Product Goal | ✅ Implementada |
| HU-019 | Crear elemento de backlog | ✅ Implementada |
| HU-020 | Editar elemento | ✅ Implementada |
| HU-022 | Ordenar/priorizar backlog | ✅ Implementada |
| HU-029 | Crear historia (Como…, quiero…, para…) | ✅ Implementada |
| HU-030 | Criterios de aceptación | ✅ Implementada |
| HU-031 | Prioridad | ✅ Implementada |
| HU-043 | Estimación en Story Points | ✅ Implementada |

### Persona 2 — Sprint, Sprint Backlog y Tablero (9 HU)
| HU | Descripción | Estado |
|----|--------------|--------|
| HU-049 | Crear Sprint | ✅ Implementada |
| HU-052 | Sprint Goal | ✅ Implementada |
| HU-058 | Seleccionar historias del backlog | ✅ Implementada |
| HU-061 | Conformar Sprint Backlog | ✅ Implementada |
| HU-054 | Cerrar Sprint | ✅ Implementada |
| HU-062 | Crear tarea | ✅ Implementada |
| HU-063 | Asignar responsable | ✅ Implementada |
| HU-068 | Tablero visual (estados) | ✅ Implementada |
| HU-069 | Mover tarjetas entre estados | ✅ Implementada |

### Persona 3 — Daily, DoD, Review, Retrospective e indicador (14 HU)
| HU | Descripción | Estado |
|----|--------------|--------|
| HU-074 | Registrar Daily Scrum | ✅ Implementada |
| HU-078 | Registrar impedimento | ✅ Implementada |
| HU-081 | Estado del impedimento | ✅ Implementada |
| HU-084 | Crear Definition of Done | ✅ Implementada |
| HU-085 | Checklist DoD | ✅ Implementada |
| HU-086 | Impedir cierre incompleto | ✅ Implementada |
| HU-098 | Mostrar incremento | ✅ Implementada |
| HU-099 | Validar historias | ✅ Implementada |
| HU-100 | Registrar feedback | ✅ Implementada |
| HU-103 | Crear retrospectiva | ✅ Implementada |
| HU-105 | Registrar problemas | ✅ Implementada |
| HU-106 | Proponer acciones de mejora | ✅ Implementada |
| HU-112 | Burndown Chart | ✅ Implementada |
| HU-116 | Cumplimiento del Sprint (% avance) | ✅ Implementada |

**Total: 36/36 HU implementadas (100%)**

##  Evidencia del Sprint

>  Completar con datos reales del Sprint ejecutado en el sistema (capturas de pantalla o texto exportado desde la app).

- **Sprint Goal:** `[completar — texto exacto ingresado en el sistema]`
- **Sprint Backlog:** `[completar — listado de historias seleccionadas para el sprint, con screenshot del sistema]`
- **Tablero:** `[adjuntar screenshot del tablero con tarjetas en sus distintos estados]`
- **Definition of Done:** `[adjuntar screenshot del checklist DoD aplicado a al menos una historia]`
- **Sprint Review:** `[adjuntar screenshot con incremento mostrado + feedback registrado]`
- **Retrospective:** `[adjuntar screenshot con ítems positivo/problema/acción registrados]`
- **Burndown:** `[adjuntar screenshot del gráfico con el % de cumplimiento final]`

##  Equipo

- Melissa — Persona 1 (Fundación, equipo, Product Backlog)
- Ziulianni — Persona 2 (Sprint, Sprint Backlog, Tablero)
- Javier — Persona 3 (Daily, DoD, Review, Retrospective, Burndown)
