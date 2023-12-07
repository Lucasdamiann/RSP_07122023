using Entidades.DataBase;
using Entidades.Exceptions;
using Entidades.Files;
using Entidades.Interfaces;
using System.Diagnostics.Tracing;

namespace Entidades.Modelos
{
    public delegate double DelegadoDemoraAtencion(double demora);
    public delegate IComestible DelegadoPedidoEnCurso(IComestible menu);

    public class Cocinero<T> where T : Hamburguesa
    {
        public event DelegadoDemoraAtencion OnDemora;
        public event DelegadoPedidoEnCurso OnPedido;
        private int cantPedidosFinalizados;
        private string nombre;
        private double demoraPreparacionTotal;
        private CancellationTokenSource cancellation;
        private T pedidosEnPreparacion;
        private Task tarea;
        private Mozo<T> mozo;
        private Queue<T> pedidos;

        public Cocinero(string nombre)
        {
            this.mozo = new Mozo<T>();
            this.pedidos = new Queue<T>();
            this.nombre = nombre;
            this.mozo.OnPedido += TomarNuevoPedido;
        }

        //No hacer nada
        public bool HabilitarCocina
        {
            get
            {
                return this.tarea is not null && (this.tarea.Status == TaskStatus.Running ||
                    this.tarea.Status == TaskStatus.WaitingToRun ||
                    this.tarea.Status == TaskStatus.WaitingForActivation);
            }
            set
            {
                if (value && !this.HabilitarCocina)
                {
                    this.mozo.EmpezarATrabajar = true;
                    this.cancellation = new CancellationTokenSource();
                    this.EmpezarACocinar();
                }
                else
                {
                    this.mozo.EmpezarATrabajar = false;
                    this.cancellation.Cancel();
                }
            }
        }

        //no hacer nada
        public double TiempoMedioDePreparacion { get => this.cantPedidosFinalizados == 0 ? 0 : this.demoraPreparacionTotal / this.cantPedidosFinalizados; }
        public string Nombre { get => nombre; }
        public int CantPedidosFinalizados { get => cantPedidosFinalizados; }
        public Queue<T> Pedidos { get; }

        private void EmpezarACocinar()
        {
            tarea = Task.Run(() =>
            {
                while (!this.cancellation.IsCancellationRequested && this.pedidos.Count > 0)
                {
                    this.pedidosEnPreparacion = pedidos.First();
                    if (this.OnPedido is not null)
                    {
                        this.OnPedido.Invoke(this.pedidosEnPreparacion);
                    }
                    this.EsperarProximoIngreso();
                    this.cantPedidosFinalizados++;
                    DataBaseManager.GuardarTicket(this.Nombre, this.pedidosEnPreparacion);
                }
            });
        }

        private void EsperarProximoIngreso()
        {
            int tiempoEspera = 0;
            while (!this.cancellation.IsCancellationRequested && pedidosEnPreparacion.Estado == false)
            {
                if (this.OnDemora is not null)
                {
                    this.OnDemora.Invoke(tiempoEspera);
                }
                Thread.Sleep(1000);
                tiempoEspera++;
            }
            this.demoraPreparacionTotal += tiempoEspera;
        }

        private void TomarNuevoPedido(T menu)
        {
            if (this.OnPedido is not null)
            {
                this.pedidos.Enqueue(menu); 
            }
        }
    }
}
