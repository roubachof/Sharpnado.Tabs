// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SillyFrontService.cs" company="The Silly Company">
//   The Silly Company 2016. All rights reserved.
// </copyright>
// <summary>
//   The SillyFrontService interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------


using MauiSample.Infrastructure;

namespace MauiSample.Domain.Silly
{
    public class SillyDudeService : ISillyDudeService
    {
        private readonly ErrorEmulator _errorEmulator;
        private readonly int _peopleCounter = 0;
        private readonly Dictionary<int, SillyDude> _repository;

        private readonly Random _randomizer = new Random();

        public SillyDudeService(ErrorEmulator errorEmulator)
        {
            _errorEmulator = errorEmulator;
            _repository = new Dictionary<int, SillyDude>
            {
                {
                    ++_peopleCounter, new SillyDude(
                        _peopleCounter,
                        "Will Ferrell",
                        "Actor",
                        "Hey.\nThey laughed at Louis Armstrong when he said he was gonna go to the moon.\nNow he’s up there, laughing at them.",
#if LOCAL_DATA
                            "will_ferrell.jpg",
#else
                        "https://news.usc.edu/files/2017/03/Will-Ferrell-Step-Brothers_Horizontal_web-824x549.jpg",
#endif
                        4,
                        "N/A",
                        "https://sayingimages.com/wp-content/uploads/dear-monday-will-ferrell-memes.jpg",
                        "https://youtu.be/sPFRZP4qY7I?t=26")
                },
                {
                    ++_peopleCounter, new SillyDude(
                        _peopleCounter,
                        "Knights of Ni",
                        "Knights",
                        "Keepers of the sacred words 'Ni', 'Peng', and 'Neee-Wom'",
#if LOCAL_DATA
                            "knights_of_ni.jpg",
#else
                        "https://upload.wikimedia.org/wikipedia/en/e/eb/Knightni.jpg",
#endif
                        5,
                        "ni!",
                        "https://i.imgflip.com/1fyfpo.jpg",
                        "https://www.youtube.com/watch?v=zIV4poUZAQo")
                },
                {
                    ++_peopleCounter, new SillyDude(
                        _peopleCounter,
                        "Jean-Claude",
                        "Actor",
                        "J’adore les cacahuètes.\nTu bois une bière et tu en as marre du goût. Alors tu manges des cacahuètes.\nLes cacahuètes, c’est doux et salé, fort et tendre, comme une femme. Manger des cacahuètes. It’s a really strong feeling.\nEt après tu as de nouveau envie de boire une bière.\nLes cacahuètes, c’est le mouvement perpétuel à la portée de l’homme.",
#if LOCAL_DATA
                            "jean_claude_van_damme.jpg",
#else
                        "https://m.media-amazon.com/images/M/MV5BNjU1NzVkOWMtYmJjYy00M2UwLTkxYmEtNmU0YjI5M2ZhYTU3XkEyXkFqcGdeQXVyMjUyNDk2ODc@._V1_.jpg",
#endif
                        5,
                        "N/A",
                        "https://www.planet.fr/files/styles/pano_xxl/public/images/diaporama/5/8/0/1615085/vignette-focus_3.jpg?itok=zMC9zW1c")
                },
                {
                    ++_peopleCounter, new SillyDude(
                        _peopleCounter,
                        "Triumph",
                        "Insult Comic Dog",
                        "Occupy Wall Street, talking to a trader with a Fox News mustache on.\nThese protesters with their whining and crying right?\nDon't they realize that their public education are being funded from the taxes you evade each year?\nI don't want to keep you, you're a good man! You better hurry back from lunch so you can collect your hurry back from lunch bonus.",
#if LOCAL_DATA
                            "louis_ck.jpg",
#else
                        "https://tonyhellerakastevengoddardisnotasociopath.files.wordpress.com/2014/09/triumph-the-insult-comic-dog.jpg",
#endif
                        2,
                        "N/A",
                        "https://i.imgflip.com/xi0tk.jpg",
                        "https://youtu.be/O-253uBJap8?t=315")
                },
                {
                    ++_peopleCounter, new SillyDude(
                        _peopleCounter,
                        "Ricky Gervais",
                        "Comedian",
                        "Science seeks the truth. And it does not discriminate. For better or worse it finds things out.\nScience is humble.\nIt knows what it knows and it knows what it doesn’t know. It bases its conclusions and beliefs on hard evidence -­- evidence that is constantly updated and upgraded.\nIt doesn’t get offended when new facts come along. It embraces the body of knowledge.\nIt doesn’t hold on to medieval practices because they are tradition.",
#if LOCAL_DATA
                            "louis_ck.jpg",
#else
                        "https://resize-parismatch.lanmedia.fr/rcrop/250,250/img/var/news/storage/images/paris-match/people-a-z/ricky-gervais/6126908-7-fre-FR/Ricky-Gervais.jpg",
#endif
                        3,
                        "N/A",
                        "https://pics.me.me/what-can-be-more-arrogant-than-believing-that-the-same-13060011.png")
                },
                {
                    ++_peopleCounter, new SillyDude(
                        _peopleCounter,
                        "Steve Carell",
                        "Comedian",
                        "Vincent van Gogh. Everyone told him: \"You only have one ear. You cannot be a great artist.\"\nAnd you know what he said?\n\"I can\'t hear you.",
#if LOCAL_DATA
                            "louis_ck.jpg",
#else
                        "https://pixel.nymag.com/imgs/daily/vulture/2018/12/22/23-brick.w330.h330.jpg",
#endif
                        3,
                        "N/A",
                        "https://sayingimages.com/wp-content/uploads/fool-me-once-michael-scott-memes-1.jpg",
                        "https://youtu.be/N9Z4vqysxMQ?t=92")
                },
                {
                    ++_peopleCounter, new SillyDude(
                        _peopleCounter,
                        "Father Ted",
                        "Priest",
                        "My Lovely Horse,\r\nRunning through the field,\r\nWhere are you going,\r\nWith your fetlocks blowing,\r\nIn the wind.\r\n\r\n“I want to shower you with sugar lumps,\r\nAnd ride you over fences,\r\nI want to polish your hooves every single day,\r\nAnd bring you to the horse dentist.\r\n\r\n“My lovely horse,\r\nYou’re a pony no more,\r\nRunning around,\r\nWith a man on your back,\r\nLike a train in the night,\r\nLike a train in the night!”",
#if LOCAL_DATA
                            "louis_ck.jpg",
#else
                        "https://vignette.wikia.nocookie.net/fatherted/images/b/b2/Ted.jpg",
#endif
                        4,
                        "N/A",
                        "https://cdn.psychologytoday.com/sites/default/files/styles/image-article_inline_full/public/blogs/129003/2014/08/158581-164751.jpg?itok=f7HhI_lo",
                        "https://www.youtube.com/watch?v=jzYzVMcgWhg")
                },
                {
                    ++_peopleCounter, new SillyDude(
                        _peopleCounter,
                        "Moss",
                        "IT Guy",
                        "Did you see that ludicrous display last night?\nThe thing about Arsenal is, they always try to walk it in!",
#if LOCAL_DATA
                            "louis_ck.jpg",
#else
                        "https://i.ytimg.com/vi/DJMr-mLjL1s/hqdefault.jpg",
#endif
                        3,
                        "N/A",
                        "https://images1.memedroid.com/images/UPLOADED8/501f9cd68575e.jpeg",
                        "https://youtu.be/NKHyqjHqQLU?t=32")
                },
                {
                    ++_peopleCounter, new SillyDude(
                        _peopleCounter,
                        "Les Nuls",
                        "Crétins Fabuleux",
                        "Agad la té'évision é pis dors!\nAgad la té'évision é pis dors.\nAgad la té'évision é pis dors!\nAgad la té'évision é pis dors.\nAgad la té'évision é pis dors!\nAgad la té'évision é pis dors.\nAgad la té'évision é pis dors.\n",
#if LOCAL_DATA
                            "louis_ck.jpg",
#else
                        "https://www.jokeme.fr/images/les-nuls-JTN.jpg",
#endif
                        4,
                        "N/A",
                        "https://img.static-rmg.be/a/view/q75/w720/h480/2223699/un-clin-doeil-a-la-cite-de-la-peur-sur-le-site-de-2-24535-1433777643-0-dblbig-jpg.jpg",
                        "https://www.youtube.com/watch?v=lNEpFJYduto")
                },
            };

            for (int id = 1; id < 200; id++)
            {
                var dudeToClone = _repository[id];
                _repository.Add(++_peopleCounter, dudeToClone.Clone(_peopleCounter));
            }
        }

        public async Task<IReadOnlyCollection<SillyDude>> GetSillyPeople()
        {
            await Task.Delay(_errorEmulator.DefaultLoadingTime);
            if (ProcessErrorEmulator())
            {
                return new List<SillyDude>();
            }

            return _repository.Values.Take(9).ToList();
        }

        public async Task<List<SillyDude>> GetSillyPeoplePage(int pageNumber, int pageSize)
        {
            await Task.Delay(TimeSpan.FromSeconds(pageNumber > 1 ? 1 : 3));
            if (ProcessErrorEmulator())
            {
                return new List<SillyDude>();
            }

            return _repository.Values.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
        }

        public async Task<SillyDude> GetSilly(int id)
        {
            await Task.Delay(_errorEmulator.DefaultLoadingTime);
            ProcessErrorEmulator();

            return _repository[id];
        }

        public async Task<SillyDude> GetRandomSilly(int waitTime = -1)
        {
            await Task.Delay(waitTime > -1 ? TimeSpan.FromSeconds(waitTime) : _errorEmulator.DefaultLoadingTime);

            if (PlatformService.IsFoldingScreen)
            {
                return _repository[6];
            }

            int minId = _repository.Keys.Min();
            int maxId = _repository.Keys.Max();

            return _repository[_randomizer.Next(minId, maxId)];
        }

        private bool ProcessErrorEmulator()
        {
            switch (_errorEmulator.ErrorType)
            {
                case ErrorType.Unknown:
                    throw new InvalidOperationException();
                case ErrorType.Network:
                    throw new NetworkException();
                case ErrorType.Server:
                    throw new ServerException();
                case ErrorType.NoData:
                    return true;
                default:
                    return false;
            }
        }
    }
}
