import { TrendingUp, Clock, Users, Shield, BarChart3, Zap } from "lucide-react";

const benefits = [
  {
    icon: Clock,
    title: "Respuesta más rápida",
    description: "Reduce el tiempo de atención a incidentes en un 60% con alertas en tiempo real.",
    color: "primary",
  },
  {
    icon: BarChart3,
    title: "Decisiones basadas en datos",
    description: "Identifica patrones y zonas críticas para optimizar recursos y presupuesto.",
    color: "info",
  },
  {
    icon: Users,
    title: "Participación ciudadana",
    description: "Involucra a la comunidad en el cuidado de la ciudad de forma simple.",
    color: "accent",
  },
  {
    icon: Shield,
    title: "Transparencia gubernamental",
    description: "Demuestra gestión proactiva y genera confianza con reportes públicos.",
    color: "primary",
  },
  {
    icon: TrendingUp,
    title: "Análisis predictivo",
    description: "Anticipa problemas antes de que escalen usando inteligencia artificial.",
    color: "warning",
  },
  {
    icon: Zap,
    title: "Integración sencilla",
    description: "Compatible con sistemas existentes. Implementación en menos de 48 horas.",
    color: "success",
  },
];

const Benefits = () => {
  return (
    <section id="beneficios" className="py-16 md:py-24">
      <div className="container mx-auto px-4">
        {/* Section Header */}
        <div className="max-w-2xl mx-auto text-center mb-12 md:mb-16">
          <h2 className="text-3xl md:text-4xl font-bold text-foreground mb-4">
            Beneficios para tu <span className="text-primary">Gobierno</span>
          </h2>
          <p className="text-lg text-muted-foreground">
            UrbanIA transforma la gestión urbana con tecnología accesible y resultados medibles.
          </p>
        </div>

        {/* Benefits Grid */}
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6 md:gap-8">
          {benefits.map((benefit, index) => (
            <div
              key={benefit.title}
              className="p-6 rounded-xl bg-card border-2 border-border shadow-soft hover:shadow-elevated hover:border-primary/30 transition-all duration-300 group"
              style={{ animationDelay: `${index * 0.1}s` }}
            >
              {/* Icon */}
              <div className={`w-14 h-14 rounded-xl flex items-center justify-center mb-5 transition-transform duration-300 group-hover:scale-110 ${
                benefit.color === "primary" ? "bg-primary/15" :
                benefit.color === "accent" ? "bg-accent/15" :
                benefit.color === "info" ? "bg-info/15" :
                benefit.color === "warning" ? "bg-warning/15" :
                "bg-success/15"
              }`}>
                <benefit.icon className={`w-7 h-7 ${
                  benefit.color === "primary" ? "text-primary" :
                  benefit.color === "accent" ? "text-accent" :
                  benefit.color === "info" ? "text-info" :
                  benefit.color === "warning" ? "text-warning" :
                  "text-success"
                }`} strokeWidth={2.5} />
              </div>

              {/* Content */}
              <h3 className="text-xl font-bold text-foreground mb-3">
                {benefit.title}
              </h3>
              <p className="text-sm text-muted-foreground leading-relaxed">
                {benefit.description}
              </p>
            </div>
          ))}
        </div>
      </div>
    </section>
  );
};

export default Benefits;
