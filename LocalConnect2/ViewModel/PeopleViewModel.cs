using System.Collections.Generic;

namespace LocalConnect2.ViewModel
{
    public class Pearson
    {
        public string FirstName { set; get; }
        public string Surname { set; get; }
        public string Description { set; get; }

        public string Name => FirstName + " " + Surname;
    }


    public class PeopleViewModel : MainViewModel
    {
        public List<Pearson> People { set; get; }

        public PeopleViewModel()
        {
            People = new List<Pearson>
            {
                new Pearson
                {
                    FirstName = "Maxwell",
                    Surname = "Baer",
                    Description = "This is simple description"
                },
                new Pearson
                {
                    FirstName = "Evangeline",
                    Surname = "Crosswell",
                    Description = "This is another simple description"
                },
                new Pearson
                {
                    FirstName = "Ericka",
                    Surname = "Fischetti",
                    Description = "This is simple description 3"
                },
                new Pearson
                {
                    FirstName = "Maxwell",
                    Surname = "Baer",
                    Description = ""
                },
                new Pearson
                {
                    FirstName = "Devona",
                    Surname = "Marlett",
                    Description = "This is simple description"
                },
                new Pearson
                {
                    FirstName = "Maranda",
                    Surname = "Baer",
                    Description = "This is simple"
                },
                new Pearson
                {
                    FirstName = "Maxwell",
                    Surname = "Macdougall",
                    Description = "This is simple description"
                },
                new Pearson
                {
                    FirstName = "Maxwell",
                    Surname = "Baer",
                    Description = "This is simple description. This is simple description. This is simple description"
                },
                new Pearson
                {
                    FirstName = "Maxwell",
                    Surname = "Michener",
                    Description = "This is simple description"
                },
            };
        } 
    }
}