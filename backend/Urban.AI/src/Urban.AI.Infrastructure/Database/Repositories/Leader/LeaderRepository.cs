namespace Urban.AI.Infrastructure.Database.Repositories.Leader;

#region Usings
using Urban.AI.Domain.Leaders;
using Microsoft.EntityFrameworkCore;
using Urban.AI.Domain.Common.Abstractions;
#endregion

internal sealed class LeaderRepository : Repository<Leader>, ILeaderRepository
{
    public LeaderRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<Leader?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await _dbContext.Set<Leader>()
            .FirstOrDefaultAsync(leader => leader.UserId == userId, cancellationToken);
    }

    public async Task<Leader?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbContext.Set<Leader>()
            .Include(l => l.User)
            .Include(l => l.Department)
            .Include(l => l.Municipality)
            .FirstOrDefaultAsync(l => l.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Leader>> GetAllActiveLeadersAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.Set<Leader>()
            .Include(l => l.User)
            .Include(l => l.Department)
            .Include(l => l.Municipality)
            .Where(l => l.IsActive)
            .ToListAsync(cancellationToken);
    }

    public async Task<Leader?> GetNearestLeaderAsync(decimal latitude, decimal longitude, CancellationToken cancellationToken)
    {
        var leaders = await _dbContext.Set<Leader>()
            .Where(l => l.IsActive)
            .ToListAsync(cancellationToken);

        if (!leaders.Any())
        {
            return null;
        }

        return leaders
            .Select(l => new
            {
                Leader = l,
                Distance = CalculateHaversineDistance(latitude, longitude, l.Latitude, l.Longitude)
            })
            .OrderBy(x => x.Distance)
            .Select(x => x.Leader)
            .FirstOrDefault();
    }

    private static double CalculateHaversineDistance(decimal lat1, decimal lon1, decimal lat2, decimal lon2)
    {
        const double earthRadiusKm = 6371.0;

        var dLat = DegreesToRadians((double)(lat2 - lat1));
        var dLon = DegreesToRadians((double)(lon2 - lon1));

        var lat1Rad = DegreesToRadians((double)lat1);
        var lat2Rad = DegreesToRadians((double)lat2);

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2) * Math.Cos(lat1Rad) * Math.Cos(lat2Rad);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return earthRadiusKm * c;
    }

    private static double DegreesToRadians(double degrees)
    {
        return degrees * Math.PI / 180.0;
    }

    public async Task<(IEnumerable<Leader> Leaders, int TotalCount)> GetLeadersWithPaginationAsync(
        int pageNumber,
        int pageSize,
        Guid? departmentId,
        Guid? municipalityId,
        CancellationToken cancellationToken)
    {
        var query = _dbContext.Set<Leader>()
            .Include(l => l.User)
            .Include(l => l.Department)
            .Include(l => l.Municipality)
            .AsQueryable();

        if (departmentId.HasValue)
        {
            query = query.Where(l => l.DepartmentId == departmentId.Value);
        }

        if (municipalityId.HasValue)
        {
            query = query.Where(l => l.MunicipalityId == municipalityId.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var leaders = await query
            .OrderBy(l => l.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (leaders, totalCount);
    }
}
