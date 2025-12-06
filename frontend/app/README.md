# UrbanAI - Sistema de Registro de Incidentes Urbanos

Aplicación web moderna para registrar y gestionar incidentes en la ciudad. Desarrollada con Next.js, TypeScript, shadcn/ui y NextAuth.

## Características

- 🔐 **Autenticación completa**: Login y registro de usuarios
- 📍 **Geolocalización**: Registro automático de ubicación del incidente
- 📸 **Subida de fotos**: Adjuntar imágenes a los reportes
- 🎨 **Interfaz moderna**: Diseño oscuro con shadcn/ui
- 📱 **Responsive**: Optimizado para todos los dispositivos
- 🔒 **Rutas protegidas**: Middleware para proteger el dashboard

## Tecnologías

- **Next.js 16** - Framework React
- **TypeScript** - Tipado estático
- **NextAuth.js** - Autenticación
- **shadcn/ui** - Componentes UI
- **Tailwind CSS** - Estilos
- **React Hook Form** - Manejo de formularios
- **Zod** - Validación de esquemas

## Instalación

1. Instala las dependencias:

```bash
npm install
```

2. Crea un archivo `.env.local` en la raíz del proyecto:

```env
NEXTAUTH_URL=http://localhost:3000
NEXTAUTH_SECRET=tu-clave-secreta-aqui
```

3. Ejecuta el servidor de desarrollo:

```bash
npm run dev
```

4. Abre [http://localhost:3000](http://localhost:3000) en tu navegador.

## Estructura del Proyecto

```
src/
├── app/
│   ├── api/
│   │   ├── auth/
│   │   │   ├── [...nextauth]/route.ts  # Configuración NextAuth
│   │   │   └── register/route.ts       # Endpoint de registro
│   │   └── incidents/route.ts          # API de incidentes
│   ├── dashboard/                       # Página del dashboard
│   ├── login/                           # Página de login
│   ├── register/                        # Página de registro
│   ├── layout.tsx                       # Layout principal
│   ├── page.tsx                         # Página de inicio (redirige a login)
│   └── providers.tsx                    # Providers de NextAuth
├── components/
│   └── ui/                              # Componentes shadcn/ui
│       ├── button.tsx
│       ├── card.tsx
│       ├── input.tsx
│       ├── label.tsx
│       └── textarea.tsx
├── lib/
│   ├── db.ts                            # Simulación de base de datos
│   └── utils.ts                         # Utilidades
└── types/
    └── next-auth.d.ts                   # Tipos de NextAuth
```

## Uso

### Usuario Demo

Para probar la aplicación, puedes usar las siguientes credenciales:

- **Email**: demo@urbanai.com
- **Contraseña**: demo123

### Registrar un Incidente

1. Inicia sesión o crea una cuenta
2. En el dashboard, completa el formulario:
   - Describe el incidente
   - La ubicación se obtiene automáticamente (permite acceso a la ubicación)
   - Opcionalmente, sube una foto
3. Haz clic en "Enviar Incidente"

## Notas Importantes

- **Base de datos**: Actualmente usa almacenamiento en memoria. Los datos se perderán al reiniciar el servidor. Para producción, se recomienda implementar Prisma con PostgreSQL o MySQL.
- **Almacenamiento de fotos**: Las fotos se almacenan temporalmente. Para producción, implementa un servicio como Cloudinary, AWS S3, o similar.
- **Autenticación**: La autenticación está simplificada para desarrollo. En producción, implementa validación completa de contraseñas con bcrypt.

## Próximos Pasos

- [ ] Integrar base de datos real (Prisma + PostgreSQL)
- [ ] Implementar almacenamiento de imágenes (Cloudinary/S3)
- [ ] Agregar mapa interactivo para visualizar incidentes
- [ ] Sistema de categorías de incidentes
- [ ] Notificaciones en tiempo real
- [ ] Panel de administración
- [ ] API pública para consultar incidentes

## Desarrollo

```bash
# Desarrollo
npm run dev

# Build de producción
npm run build

# Iniciar producción
npm start

# Linting
npm run lint
```

## Licencia

Este proyecto es de código abierto y está disponible bajo la licencia MIT.
