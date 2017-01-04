using TAL.QuoteAndApply.Search.LuceneWrapper;

namespace TAL.QuoteAndApply.Search.Models
{
    public class QuestionAnswerDocument : ADocument
    {
        private string _name;
        private string _similies;

        [SearchField]
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                AddParameterToDocumentNoStoreParameter("Name", _name);
            }
        }

        [SearchField]
        public string Similies
        {
            get { return _similies; }
            set
            {
                _similies = value;
                AddParameterToDocumentNoStoreParameter("Similies", _similies);
            }
        }
    }
}
