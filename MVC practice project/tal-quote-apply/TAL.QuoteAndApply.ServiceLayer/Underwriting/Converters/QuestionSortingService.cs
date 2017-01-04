using System;
using System.Collections.Generic;
using System.Linq;
using TAL.QuoteAndApply.Underwriting.Models.Dto;

namespace TAL.QuoteAndApply.ServiceLayer.Underwriting.Converters
{
    public static class QuestionSortingService
    {
        public static IEnumerable<ReadOnlyQuestion> Sort(IEnumerable<ReadOnlyCategory> categories, IEnumerable<ReadOnlyQuestion> questions)
        {
            var unOrderedQuestions = new List<ReadOnlyQuestion>(questions);

            var orderedQuestions = new List<ReadOnlyQuestion>();

            //Use the base level questions to seed the ordered list of questions
            foreach (var category in categories.OrderBy(c => c.OrderId))
            {
                var categoryId = category.Id;

                var categoryQuestions = unOrderedQuestions.Where(q => q.ParentId == categoryId);

                orderedQuestions.AddRange(categoryQuestions.OrderBy(q => q.OrderId));
            }

            //Clean out the ordered questions from the unOrderedList
            foreach (var orderedQuestion in orderedQuestions)
            {
                unOrderedQuestions.Remove(orderedQuestion);
            }

            //Iterate through the list until we have all the questions from the list
            while (unOrderedQuestions.Count > 0)
            {
                //This is a sanity check. If the number of ordered questions is the same as before and after then no questions are
                //being processed leading to an inifite loop
                var checkSum = unOrderedQuestions.Count;

                //Group each remaining question by their parent
                var groupedQuestions = unOrderedQuestions.GroupBy(q => q.ParentId);

                foreach (var groupedQuestion in groupedQuestions)
                {
                    var orderedGroupedQuestions = groupedQuestion.OrderBy(q => q.OrderId);

                    var parentInOrderedList =
                        orderedQuestions.SingleOrDefault(q => q.Answers.Any(a => a.SelectedId == groupedQuestion.Key));

                    //A parent may actually come after a child. Just proceed on and catch it in a future round
                    if (parentInOrderedList != null)
                    {
                        var parentIndex = orderedQuestions.IndexOf(parentInOrderedList);

                        //Insert after the parent
                        orderedQuestions.InsertRange(parentIndex + 1, orderedGroupedQuestions);

                        //Clean out the ordered questions from the unOrderedList
                        foreach (var orderedQuestion in orderedGroupedQuestions)
                        {
                            unOrderedQuestions.Remove(orderedQuestion);
                        }
                    }
                }

                //No questions were matched to a parent in this round, and they won't be in any future round.
                if (checkSum == unOrderedQuestions.Count)
                {
                    var orphanedQuestions = string.Join(",", unOrderedQuestions.Select(q => q.Id));

                    throw new ApplicationException($"Issues sorting interview. No parents found for the following questions: {orphanedQuestions}");
                }
            }

            return orderedQuestions;
        }
    }
}