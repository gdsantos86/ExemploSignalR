using Microsoft.AspNetCore.SignalR;

namespace ExemploSignalR.Hubs
{
    public class VendasHub : Hub
    {
        // Método que pode ser chamado pelos clientes para notificar uma nova venda.
        public async Task NotificaTotalVendas(int totalVendas)
        {
            await Clients.All.SendAsync("RecebeTotalVendas", totalVendas);
        }


    }
}
