using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ExemploSignalR.Data;
using ExemploSignalR.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.CodeAnalysis;
using ExemploSignalR.Hubs;
using Microsoft.AspNetCore.SignalR.Client;

namespace ExemploSignalR.Controllers
{
    public class VendasController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IHubContext<VendasHub> _hubContext;



        public VendasController(AppDbContext context, IHubContext<VendasHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }



        // GET: Vendas
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Vendas.Include(v => v.Produto);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Vendas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var venda = await _context.Vendas
                .Include(v => v.Produto)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (venda == null)
            {
                return NotFound();
            }

            return View(venda);
        }

        // GET: Vendas/Create
        public IActionResult Create()
        {
            ViewData["ProdutoId"] = new SelectList(_context.Produtos, "Id", "Nome");
            return View();
        }

        // POST: Vendas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ProdutoId,Quantidade,DataVenda")] Venda venda)
        {
            if (ModelState.IsValid)
            {
                _context.Add(venda);
                await _context.SaveChangesAsync();

                // Recupera o novo número total de vendas para o produto
                int novoTotalVendas = _context.Vendas.Sum(s => s.Quantidade);

                // Envia a atualização para os clientes conectados
                await _hubContext.Clients.All.SendAsync("RecebeTotalVendas", novoTotalVendas);

                return RedirectToAction(nameof(Index));
            }
            ViewData["ProdutoId"] = new SelectList(_context.Produtos, "Id", "Nome", venda.ProdutoId);
            return View(venda);
        }

        // GET: Vendas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var venda = await _context.Vendas.FindAsync(id);
            if (venda == null)
            {
                return NotFound();
            }
            ViewData["ProdutoId"] = new SelectList(_context.Produtos, "Id", "Nome", venda.ProdutoId);
            return View(venda);
        }

        // POST: Vendas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProdutoId,Quantidade,DataVenda")] Venda venda)
        {
            if (id != venda.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(venda);
                    await _context.SaveChangesAsync();

                    // Adicionar notificação após edição também
                    int novoTotalVendas = _context.Vendas.Sum(s => s.Quantidade);
                    await _hubContext.Clients.All.SendAsync("RecebeTotalVendas", novoTotalVendas);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VendaExists(venda.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProdutoId"] = new SelectList(_context.Produtos, "Id", "Nome", venda.ProdutoId);
            return View(venda);
        }

        // GET: Vendas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var venda = await _context.Vendas
                .Include(v => v.Produto)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (venda == null)
            {
                return NotFound();
            }

            return View(venda);
        }

        // POST: Vendas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var venda = await _context.Vendas.FindAsync(id);
            if (venda != null)
            {
                _context.Vendas.Remove(venda);
            }

            await _context.SaveChangesAsync();

            // Notificar após exclusão também
            int novoTotalVendas = _context.Vendas.Sum(s => s.Quantidade);
            await _hubContext.Clients.All.SendAsync("RecebeTotalVendas", novoTotalVendas);

            return RedirectToAction(nameof(Index));
        }

        private bool VendaExists(int id)
        {
            return _context.Vendas.Any(e => e.Id == id);
        }
    }
}
