using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotesTracker
{
    class Note
    {
        public long Id;
        public string Title;
        public string Author;
        public string Adresat;
        public DateTime CreateDate;
        public DateTime FinishDate;


        public Note()
        {
            Id = 0;
            Title = "No title";
            Author = "No author";
            Adresat = "No adresat";
            
        }

        public Note (long id, string title, string author, string adresat, DateTime createDate, DateTime finishDate)
        {
            Id = id;
            Title = title;
            Author = author;
            Adresat = adresat;
            CreateDate = createDate;
            FinishDate = finishDate;
        }
            
    }
}
