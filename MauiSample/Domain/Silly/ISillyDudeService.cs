namespace MauiSample.Domain.Silly
{
    public interface ISillyDudeService
    {
        Task<IReadOnlyCollection<SillyDude>> GetSillyPeople();

        Task<List<SillyDude>> GetSillyPeoplePage(int pageNumber, int pageSize);

        Task<SillyDude> GetSilly(int id);

        Task<SillyDude> GetRandomSilly(int waitTime = 2);
    }
}