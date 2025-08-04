using Bogus;
using System.Globalization;

namespace Task5Web.Models
{
    public class BookGenerator
    {
        public List<BookModel> GeneratePage(int userSeed, int pageNumber, string locale,
            double avgLikes, double avgReviews)
        {
            var books = new List<BookModel>();

            // Calculate page boundaries
            int startRecord = pageNumber == 1 ? 1 : 21 + ((pageNumber - 2) * 10);
            int recordCount = pageNumber == 1 ? 20 : 10;

            for (int i = 0; i < recordCount; i++)
            {
                int recordIndex = startRecord + i;
                var book = GenerateBook(userSeed, recordIndex, locale, avgLikes, avgReviews);
                books.Add(book);
            }

            return books;
        }

        private BookModel GenerateBook(int userSeed, int recordIndex, string locale,
            double avgLikes, double avgReviews)
        {
            return new BookModel
            {
                Index = recordIndex,
                ISBN = GenerateISBN(userSeed, recordIndex),
                Title = GenerateTitle(userSeed, recordIndex, locale),
                Authors = GenerateAuthors(userSeed, recordIndex, locale),
                Publisher = GeneratePublisher(userSeed, recordIndex, locale),
                Likes = GenerateFractionalCount(avgLikes, userSeed, recordIndex, "likes"),
                Reviews = GenerateReviews(userSeed, recordIndex, avgReviews, locale)
            };
        }

        private string GenerateISBN(int userSeed, int recordIndex)
        {
            var seed = HashCode.Combine(userSeed, recordIndex, "isbn");
            var faker = new Faker();
            Randomizer.Seed = new Random(seed);
            var ean = faker.Commerce.Ean13();
            return $"978-{ean.Substring(3, 1)}-{ean.Substring(4, 3)}-{ean.Substring(7, 5)}-{ean.Substring(12, 1)}";
        }

        private string GenerateTitle(int userSeed, int recordIndex, string locale)
        {
            var seed = HashCode.Combine(userSeed, recordIndex, "title");
            var faker = new Faker(locale);
            Randomizer.Seed = new Random(seed);

            return BookTitle(faker, locale);
        }

        private string BookTitle(Faker faker, string lang)
        {
            var templates = TitleTemplates.ContainsKey(lang) ? TitleTemplates[lang] : TitleTemplates["en"];

            string adjective = faker.Commerce.ProductAdjective();
            string country = faker.Address.Country();
            string city = faker.Address.City();
            string material = faker.Commerce.ProductMaterial();

            string rawTemplate = faker.PickRandom(templates);
            return string.Format(rawTemplate, adjective, country, city, material);
        }
        
        private List<string> GenerateAuthors(int userSeed, int recordIndex, string locale)
        {
            var seed = HashCode.Combine(userSeed, recordIndex, "cautho");
            var faker = new Faker(locale);
            Randomizer.Seed = new Random(seed);
            var authorCount = faker.Random.Int(1, 3);
            var authors = new List<string>();
            for (int i = 0; i < authorCount; i++)
            {
                var authorseed = HashCode.Combine(seed, i);
                var authorFaker = new Faker(locale);
                Randomizer.Seed = new Random(authorseed);
                authors.Add(authorFaker.Name.FullName());
            }

            return authors;
        }

        private string GeneratePublisher(int userSeed, int recordIndex, string locale)
        {
            var seed = HashCode.Combine(userSeed, recordIndex, "publisher");
            var faker = new Faker(locale);
            Randomizer.Seed = new Random(seed);

            return faker.Company.CompanyName();
        }

        private int GenerateFractionalCount(double average, int userSeed, int recordIndex, string countType)
        {
            var countSeed = HashCode.Combine(userSeed, recordIndex, countType);
            var random = new Random(countSeed);
            int baseCount = (int)Math.Floor(average);
            double fractionalPart = average - baseCount;
            if (random.NextDouble() < fractionalPart)
            {
                return baseCount + 1;
            }

            return baseCount;
        }

        private List<ReviewModel> GenerateReviews(int userSeed, int recordIndex, double avgReviews, string locale)
        {
            var reviewCount = GenerateFractionalCount(avgReviews, userSeed, recordIndex, "reviewCount");

            if (reviewCount == 0)
                return new List<ReviewModel>();

            var reviewSeed = HashCode.Combine(userSeed, recordIndex, "reviews");
            var reviews = new List<ReviewModel>();

            for (int i = 0; i < reviewCount; i++)
            {
                var individualReviewSeed = HashCode.Combine(reviewSeed, i);
                var faker = new Faker(locale);
                var random = new Random(individualReviewSeed);

                reviews.Add(new ReviewModel
                {
                    ReviewText = Review(faker, locale),
                    ReviewerName = faker.Name.FullName()
                });
            }

            return reviews;
        }
        private string Review(Faker faker, string lang)
        {
            var templates = ReviewTemplates.ContainsKey(lang) ? ReviewTemplates[lang] : ReviewTemplates["en"];

            string sentiment = faker.Commerce.ProductAdjective();
            string item = faker.Commerce.Product();
            string author = faker.Name.FullName();
            string time = faker.Date.Recent(60).ToString("MMMM yyyy", new CultureInfo(lang));
            string genre = faker.Commerce.Categories(1)[0];

            string template = faker.PickRandom(templates);
            return string.Format(template, sentiment, item, author, time, genre);

        }

        private static readonly Dictionary<string, string[]> TitleTemplates = new()
        {
            ["es"] = new[]
            {
                "El siglo {0}", "Una historia de {1}", "El auge de {2}",
                "Crónicas de la era {0}", "La revolución {0}",
                "La antigua {1}: una perspectiva {0}",
                "La caída del imperio {0}", "Guerra y paz en {1}",
                "La era del descubrimiento {0}", "Imperios de {3}",
                "Las crónicas de {1}", "El legado de la dinastía {0}"
            },
            ["en"] = new[]
            {
                "The {0} Century", "A History of {1}", "The Rise of {2}",
                "Chronicles of the {0} Era", "The {0} Revolution",
                "Ancient {1}: A {0} Perspective", "The Fall of the {0} Empire",
                "War and Peace in {1}", "The {0} Age of Discovery",
                "Empires of {3}", "The {1} Chronicles", "Legacy of the {0} Dynasty"
            },
            ["fr"] = new[]
            {
                "Le siècle {0}",
                "Une histoire de {1}",
                "L'essor de {2}",
                "Chroniques de l'ère {0}",
                "La révolution {0}",
                "{1} ancien : une perspective {0}",
                "La chute de l'empire {0}",
                "Guerre et paix à {1}",
                "L'âge des découvertes {0}",
                "Empires de {3}",
                "Les chroniques de {1}",
                "L'héritage de la dynastie {0}"
            }
        };
        private static readonly Dictionary<string, string[]> ReviewTemplates = new()
        {
            ["en"] = new[]
                {
                    "A {0} {1} by {2}, read {3}. Truly an unforgettable journey through {4}.",
                    "Although the writing felt {0}, the story in this {1} by {2} kept me hooked {3}.",
                    "I found this {1} while browsing {3}, and it surprised me with its {0} tone. {4} fans will enjoy it.",
                    "The {1} crafted by {2} offers a {0} perspective on {4}. I read it {3}.",
                    "While the pacing was somewhat {0}, the {1} by {2} left an impression {3} through its unique take on {4}."
                },
            ["es"] = new[]
                {
                    "Una {1} {0} de {2}, leída {3}. Un viaje inolvidable por el mundo del {4}.",
                    "Aunque la escritura se sintió algo {0}, la historia de esta {1} de {2} me atrapó desde {3}.",
                    "Encontré esta {1} navegando {3} y me sorprendió con su tono {0}. Si te gusta el {4}, te gustará.",
                    "La {1} creada por {2} ofrece una visión {0} sobre el género {4}. La leí {3}.",
                    "A pesar de un ritmo algo {0}, la {1} de {2} me dejó huella {3} gracias a su enfoque sobre el {4}."
                },
            ["fr"] = new[]
                {
                    "Un(e) {1} {0} par {2}, lu(e) {3}. Un voyage inoubliable dans le monde du {4}.",
                    "Bien que l'écriture soit un peu {0}, l'histoire de ce(tte) {1} par {2} m'a captivé(e) dès {3}.",
                    "J'ai découvert ce(tte) {1} en flânant {3}, et son ton {0} m'a surpris(e). Les amateurs de {4} apprécieront.",
                    "Le {1} écrit par {2} propose une vision {0} du genre {4}. Lu(e) {3}.",
                    "Malgré un rythme un peu {0}, le {1} de {2} m'a laissé(e) une impression {3} avec son regard sur le {4}."
                }
        };

    }

}

