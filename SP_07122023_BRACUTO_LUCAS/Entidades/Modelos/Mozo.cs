using Entidades.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades.Modelos
{
    public delegate void DelegadoNuevoPedido<T>(T menu);
    internal class Mozo<T> where T : IComestible
    {
        public event DelegadoNuevoPedido<T> OnPedido;
        private CancellationTokenSource cancellation;
        private T menu;
        private Task tarea;

        public bool EmpezarATrabajar {
            get
            {
                return this.tarea is not null && (this.tarea.Status == TaskStatus.Running ||
                    this.tarea.Status == TaskStatus.WaitingToRun ||
                    this.tarea.Status == TaskStatus.WaitingForActivation);
            }
            set 
            {
                if (value && !this.EmpezarATrabajar)
                {
                    this.cancellation = new CancellationTokenSource();
                    this.TomarPedidos();
                }
                else
                {
                    this.cancellation.Cancel();
                }
            }
        }

        private void NotificarNuevoPedido()
        {
            if (this.OnPedido is not null)
            {
                IComestible hamburguesa = new Hamburguesa();
                this.menu = (T)hamburguesa;
                hamburguesa.IniciarPreparacion();
                this.OnPedido.Invoke(this.menu);
            }
        }

        private void TomarPedidos()
        {
            tarea = Task.Run(() => {
                while (!this.cancellation.IsCancellationRequested)
                {
                    this.NotificarNuevoPedido();
                    Thread.Sleep(5000);
                }
            });
        }
    }
}
