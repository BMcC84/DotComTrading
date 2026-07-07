using DotComTrading.Models;
using Microsoft.EntityFrameworkCore;

namespace DotComTrading.Data
{
    //Repository responsible for retrieving and persisting portfolio data
    public class PortfolioRepository
    {
        private readonly DotComTradingDBContext _context;

        public PortfolioRepository(DotComTradingDBContext context)
        {
            _context = context;
        }

        public async Task<List<Portfolio>> GetAllAsync()
        {
            return await _context.Portfolios.Include(p => p.User).ToListAsync();
        }

        public async Task<Portfolio?> GetByIdAsync(int id)
        {
            return await _context.Portfolios.Include(p => p.Holdings).Include(p => p.Trades).ThenInclude(h => h.Website).FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
