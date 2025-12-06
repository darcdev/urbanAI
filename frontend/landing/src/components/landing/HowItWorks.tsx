import { Camera, MapPin, Eye, Bell } from "lucide-react";

const steps = [
  {
    icon: Camera,
    step: "01",
    title: "El ciudadano captura",
    description: "El usuario detecta un incidente (tubería rota, accidente vehicular, bache peligroso) y toma una foto desde la aplicación.",
    color: "primary",
  },
  {
    icon: MapPin,
    step: "02",
    title: "Geolocalización automática",
    description: "El sistema obtiene automáticamente las coordenadas GPS exactas del lugar donde se tomó la fotografía.",
    color: "accent",
  },
  {
    icon: Eye,
    step: "03",
    title: "Visualización en mapa de calor",
    description: "Los incidentes se agregan al mapa de calor, mostrando las zonas con mayor concentración de reportes.",
    color: "info",
  },
  {
    icon: Bell,
    step: "04",
    title: "Notificación a autoridades",
    description: "Las entidades gubernamentales reciben alertas en tiempo real y pueden priorizar la atención según la densidad de incidentes.",
    color: "warning",
  },
];

const HowItWorks = () => {
  return (
    <section id="como-funciona" className="py-16 md:py-24">
      <div className="container mx-auto px-4">
        {/* Section Header */}
        <div className="max-w-2xl mx-auto text-center mb-12 md:mb-16">
          <h2 className="text-3xl md:text-4xl font-bold text-foreground mb-4">
            ¿Cómo <span className="text-primary">Funciona</span>?
          </h2>
          <p className="text-lg text-muted-foreground">
            Un proceso simple y eficiente que conecta a los ciudadanos con las autoridades.
          </p>
        </div>

        {/* Steps Grid */}
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 md:gap-8">
          {steps.map((step, index) => (
            <div
              key={step.step}
              className="relative p-6 rounded-xl bg-card border-2 border-border shadow-soft hover:shadow-elevated hover:border-primary/30 transition-all duration-300 group"
              style={{ animationDelay: `${index * 0.1}s` }}
            >
              {/* Step Number */}
              <span className="absolute -top-3 -right-3 w-12 h-12 rounded-full bg-primary flex items-center justify-center text-sm font-bold text-primary-foreground shadow-lg">
                {step.step}
              </span>

              {/* Icon */}
              <div className={`w-16 h-16 rounded-xl flex items-center justify-center mb-5 transition-transform duration-300 group-hover:scale-110 ${
                step.color === "primary" ? "bg-primary/15" :
                step.color === "accent" ? "bg-accent/15" :
                step.color === "info" ? "bg-info/15" :
                "bg-warning/15"
              }`}>
                <step.icon className={`w-8 h-8 ${
                  step.color === "primary" ? "text-primary" :
                  step.color === "accent" ? "text-accent" :
                  step.color === "info" ? "text-info" :
                  "text-warning"
                }`} strokeWidth={2.5} />
              </div>

              {/* Content */}
              <h3 className="text-xl font-bold text-foreground mb-3">
                {step.title}
              </h3>
              <p className="text-sm text-muted-foreground leading-relaxed">
                {step.description}
              </p>

              {/* Connector Line (hidden on last item and mobile) */}
              {index < steps.length - 1 && (
                <div className="hidden lg:block absolute top-1/2 -right-4 w-8 h-1 bg-primary/30 rounded-full" />
              )}
            </div>
          ))}
        </div>
      </div>
    </section>
  );
};

export default HowItWorks;
