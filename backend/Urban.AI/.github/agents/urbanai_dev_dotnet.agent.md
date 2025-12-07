---
description: 'Agente especializado en el desarrollo y gestión de la aplicación UrbanAI, una plataforma para reportar barreras urbanas mediante fotografías y gestionar estos reportes por diferentes roles de usuario.'
tools: ['edit/createFile', 'edit/createDirectory', 'edit/editNotebook', 'edit/editFiles', 'search', 'new', 'runCommands', 'runTasks', 'usages', 'vscodeAPI', 'problems', 'changes', 'testFailure', 'fetch', 'githubRepo', 'todos']
model: Claude Sonnet 4.5 (copilot)
---

# Descripción del agente UrbanAI Dev .NET
El objetivo de UrbanAI es ser una app que permita a ciudadanos reportar barreras urbanas mediante la toma de una fotografía como (escaleras, andenes rotos, falta de rampas, fugas de agua, falta de energía/luz) y situaciones relevantes directamente en un mapa, en donde cada fotografía será procesada por un modelo de IA que extraera toda la información relevante para categorizar el reporte. La aplicación tambien permite a lideres de localidades específicas gestionar los repostes realizados en su área, donde podrá aceptar o rechazar los reportes, asignarles un estado y priorizarlos según su gravedad.

## Roles de la aplicación
- Ciudadano: Usuario que reporta barreras urbanas mediante fotografías y descripciones (No requiere autenticación).
- Líder: Usuario que gestiona los reportes en su localidad, puede aceptar/rechazar reportes, asignar estados y prioridades (Requiere autenticación) -> Estos lideres.
- Aministrador: Usuario con permisos completos para gestionar usuarios y configuraciones del sistema (Requiere autenticación).
- Entidad: Usuario que representa una entidad gubernamental o privada que supervisa y gestiona los reportes a gran escala (Requiere autenticación).

## KeyWords: 
- **Reporte:** Incidente reportado por un ciudadano que incluye una fotografía, descripción, ubicación y estado.
- **Radicado:** Código único asignado a cada reporte para su seguimiento.
- **Estadisticas:** Reporte de datos filtrado por departamento o municipio (estos datos corresponden a los reportes de cada zona).

## Flujo de la aplicación - Rol: Administrador (Requiere autenticación)
1. El administrador inicia sesión en la aplicación web utilizando sus credenciales.
2. Una vez autenticado, el administrador accede a su panel de control donde puede agregar nuevos líderes: 
    - Proporciona un formulario para ingresar el nombre, correo electrónico, contraseña y la localidad asignada al líder (la localidad será extraida de la base de datos de georeferenciación la cual marcará la longitud y latitud a la que pertenecerá - El frontend envía al backend la localidad, el backend consulta esa localidad y devuelve al frontend la longitud y latitud asociadas a esa localización en dos campos editables por si el lider requiere modificar esos parámetros).
    - Al enviar el formulario, el backend valida los datos y crea una nueva cuenta de líder en la base de datos.
    - Al registrar un nuevo líder, el sistema envía un correo electrónico al líder con sus credenciales de acceso.
3. Una vez autenticado, el administrador también puede registrar entidades desde su panel de control:
    - Proporciona un formulario para ingresar el nombre de la entidad, correo electrónico, contraseña y descripción.
    - Al enviar el formulario, el backend valida los datos y crea una nueva cuenta de entidad en la base de datos.
    - Al registrar una nueva entidad, el sistema envía un correo electrónico a la entidad con sus credenciales de acceso.

## Flujo de la aplicación - Rol: Líder (Requiere autenticación)
1. El líder inicia sesión en la aplicación web utilizando sus credenciales.
2. Una vez autenticado, el líder accede a su panel de control donde puede ver una lista de reportes asignados a su localidad.
3. El líder puede ver los reportes aceptados, pendientes de aceptación y rechazados en diferentes secciones (solo los reportes asociados a el).
4. Al seleccionar un reporte, el líder puede ver los detalles del mismo, incluyendo la fotografía, descripción, ubicación y cualquier comentario adicional.
5. En caso de aceptar puede asignar una prioridad al reporte. El sistema debe agregar por defecto la fecha de atención al incidente (independientemente de si es rechazado o aceptado).

## Flujo de la aplicación - Rol: Ciudadano (No requiere autenticación)
1. El usuario abre la aplicación web y accede a la función de reportar.
2. Toma una fotografía del obstáculo o situación relevante en el entorno urbano.
3. Opcionalmente tiene la posibilidad de añadir una descripción o comentario adicional sobre la fotografía y un correo (opcional) en donde la aplicación notificara un código de "Radicado" para consultar posteriormente ese reporte realizado.
4. Adicionalmente la aplicación cliente obtiene la ubicación geográfica del usuario mediante el GPS del dispositivo y la envía como latitud y longitud. 
5. La fotografía es enviada al backend como FormData junto a la descripción (en caso de tenerla), correo y la ubicación.
6. El backend recibe la fotografía y la información adicional, y utiliza un modelo de IA para analizar la imagen y extraer detalles relevantes (tipo de obstáculo, gravedad, etc.).
7. El backend almacena la fotografía en minio y la información extraída por la IA en la base de datos (postgres) junto con la descripción y el correo (si aplica) y el path de la fotografía en minio.
8. Al momento de guardar el reporte en la base de datos, a este se le agrega el campo del id del municipio, el cual se obtiene del lider
9. Si en el request existe un correo, el backend envía un correo electrónico al usuario con el código de "Radicado" para que pueda consultar el estado de su reporte en el futuro.
10. El backend asigna esta incidencia reportada al lider basado en la ubicación geográfica, considerando el punto más cercano al reporte (La ubicación se determina utilizando la formula de Haversine para calcular la distancia entre dos puntos geográficos y asignarlo al lider más cercano).
11. La aplicación web muestra un mapa interactivo donde se visualizan todos los reportes realizados por los usuarios, incluyendo la fotografía, descripción y ubicación.

## Flujo de la aplicación - Rol: Entidad (Requiere autenticación)
1. La entidad inicia sesión en la aplicación web utilizando sus credenciales.
2. Una vez autenticada, la entidad accede a su panel de control donde puede ver estadísticas agregadas de los reportes en todas las localidades.
3. La entidad puede filtrar las estadísticas por departamento o municipio para analizar los datos específicos de cada zona.
4. La entidad puede generar informes descargables en formato PDF o Excel con las estadísticas de los reportes para su revisión y análisis.

