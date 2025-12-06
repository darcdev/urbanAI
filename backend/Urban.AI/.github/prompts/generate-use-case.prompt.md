---
mode: backend-reference-architecture
---
Cada vez que vaya a crear un nuevo caso de uso, empieza por crear una nueva rama en tu repositorio de GitHub. Nombra una rama basada en develop con el formato `feature/<nombre-del-caso-de-uso>`, donde `<nombre-del-caso-de-uso>` es una descripción breve y clara del caso de uso que estás implementando.

A continuación, crea un nuevo archivo en el directorio `.github/prompts/` de tu repositorio. Nombra el archivo con el formato `<nombre-del-caso-de-uso>.prompt.md`, utilizando el mismo `<nombre-del-caso-de-uso>` que usaste para la rama.

Una vez que hayas creado la rama y el archivo, abre el archivo `.github/prompts/generate-use-case.prompt.md` y copia su contenido en el nuevo archivo que acabas de crear. Este archivo contiene una plantilla que te guiará en la creación del caso de uso.

Después de copiar el contenido, edita el nuevo archivo para personalizarlo según el caso de uso específico que estás implementando. Asegúrate de proporcionar toda la información necesaria y de seguir las instrucciones en la plantilla.