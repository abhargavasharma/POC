'use strict';

/*
    Service for helping out with Underwriting
 */
angular.module('salesPortalApp').service('talUnderwritingService', ['$log', function ($log) {
    var service = this;

    var ANSWER_TYPE = {
        DEFAULT: 'Default'
    };

    var applyChangedAttributes = function(object, changedAttributes) {
        _.assign(object, changedAttributes);
    };

    this.clearAllAnswers = function(question) {
        _.forEach(question.answers, function(answer){
            answer.isSelected = false;
        });
    };

    this.setAnsweredById = function(question, answerId) {
        var answer = _.find(question.answers, {id: answerId});
        if (!answer) {
            throw 'answer not found';
        }

        answer.isSelected = true;
    };

    /**
     * Build context for sub-questions based on type (or TAG)
     * @param questionsToUpdate
     * @param contextQuestions
     */
    this.buildQuestionsContext = function (questionsToUpdate, contextQuestions) {
        $log.debug('buildQuestionsContext to-update:' + questionsToUpdate.length + ' context: ' + contextQuestions.length);

        _.each(questionsToUpdate, function (q) {
            $log.debug('uw - processing question: ' + q.id);

            if(contextQuestions.length){
                var matches = _.filter(contextQuestions, function (p) {
                    return q.id.indexOf(p.id) > -1;
                });
                q.context = '';

                if (matches.length > 0) {
                    var lastQuestion = _.last(_.sortBy(matches, function (q) { return q.id.length; }));
                    
                    $log.debug('uw - found matches: ' + lastQuestion.id + ' text: ' + lastQuestion.text);
                    q.context = lastQuestion.text;
                }
            }

            var isContext = _.some(q.tags, function (t) { return t === 'DIGITAL_ANSWER_IS_CONTEXT'; });
            if(isContext){
                $log.debug('uw - context question: ' + q.id);
                _.each(q.answers, function (a) {
                    if(a.selectedId){
                        var i = {id: a.selectedId, text: a.text};
                        contextQuestions.push(i);
                        $log.debug('uw - added answer: ' + a.selectedId);
                    }
                });
            }
        });
    };

    this.isDefaultAnswer = function (answer) {
        return answer.answerType === ANSWER_TYPE.DEFAULT;
    };

    this.isIconAnswer = function (answer) {
        return _.some(answer.tags, function (tag) {
            return tag.indexOf('SPORT_ICON_') > -1;
        });
    };

    this.createOptionForAnswer = function (answer) {
        return {
            label: answer.text,
            value: answer.id,
            classes: answer.tags.join(' ')
        };
    };

    this.mapAnswersToOptions = function (answers) {
        return _.map(answers, service.createOptionForAnswer);
    };


    this.getAllDefaultAnswers = function (question) {
        return _.filter(question.answers, {answerType: ANSWER_TYPE.DEFAULT});
    };

    this.getGroupName = function (questionId) {
        return questionId.replace(/[\s\?]/g, '_');
    };

    /**
     * Check answer type for each answer and split them into 2 groups 'Default' and other answers
     * @param question Question object that hold the answers
     * @returns {*[]} Array of 2 items: [0]=default answers. [1] other answers
     */
    this.separateDefaultAnswers = function (question) {
        var groups = _.groupBy(question.answers, function (answer) {
            return answer.answerType === ANSWER_TYPE.DEFAULT;
        });

        var defaultAnswers = _.get(groups, true);
        var otherAnswers = _.get(groups, false);

        return [defaultAnswers, otherAnswers];
    };

    this.getAllNonNoneAnswers = function(question) {
        return _.filter(question.answers, {isNoneOption:false});
    };

    this.getSingleNoneAnswer = function(question) {
        var noneAnswer = _.find(question.answers, {isNoneOption:true});

        return noneAnswer;
    };

    this.toggleAnswerSelected = function(answer) {
        answer.isSelected = !answer.isSelected;
    };

    this.hasAnySelectedAnswers = function(question) {
        return _.some(question.answers, {isSelected:true});
    };

    this.getSelectedSingleAnswer = function(question) {
        return _.find(question.answers, {isSelected: true});
    };

    this.applyRemovedQuestions = function(questions, removedQuestionIds) {

        _.forEach(removedQuestionIds, function(removedQuestionId){
            var questionIndex = _.findIndex(questions, {id:removedQuestionId});
            if (questionIndex < 0) {
                //We don't care about removed questions - changed for DE6749
                return;
            }

            questions.splice(questionIndex, 1);
        });

    };

    this.applyChangedQuestionUpdates = function(questions, changedQuestions) {
        _.forEach(changedQuestions, function(changedQuestion){
            var question = _.find(questions, {id: changedQuestion.id});
            if (!question) {
                throw 'No changed question found with id ' + changedQuestion.id;
            }

            applyChangedAttributes(question, changedQuestion.changedAttributes);

            _.forEach(changedQuestion.changedAnswers, function(changedAnswer){
                var answer = _.find(question.answers, {id:changedAnswer.id});
                if (!answer) {
                    throw 'No changed answer found with id ' + changedAnswer.id;
                }

                applyChangedAttributes(answer, changedAnswer.changedAttributes);
            });
        });
    };

    this.applyAddedQuestions = function(questions, addedQuestions) {
        _.forEach(addedQuestions, function(addedQuestion){
            questions.push(addedQuestion);
        });
    };

    this.sortQuestions = function(categories, allQuestions){
        var unOrderedQuestions  = allQuestions;
        var orderedQuestions = [];

        var orderedCategories = _.sortBy(categories, function(c) { return c.orderId; });
        _.forEach(orderedCategories, function(cat){

            var questionsForCategory = _.filter(unOrderedQuestions, {parentId: cat.id});
            var orderedQuestionsForCategory = _.sortBy(questionsForCategory, function(q) { return q.orderId; });
            orderedQuestions = orderedQuestions.concat(orderedQuestionsForCategory);
        });

        $log.debug('top level questions ordered by category');
        $log.debug(orderedQuestions);

        _.forEach(orderedQuestions, function(oq){

            var idx = _.findIndex(unOrderedQuestions, { 'id' : oq.id });

            if (idx > -1) {
                unOrderedQuestions.splice(idx, 1);
            }
            else {
                throw 'Cannot remove question from array ' + oq.id;
            }
        });
        $log.debug('remaining unordered questions');
        $log.debug(unOrderedQuestions);


        while(unOrderedQuestions.length > 0) {

            //This is a sanity check. If the number of ordered questions is the same as before and after then no questions are
            //being processed leading to an infinite loop
            var checkSum = unOrderedQuestions.length;

            //Group each remaining question by their parent
            var groupedQuestions = _.groupBy(unOrderedQuestions, 'parentId');
            $log.debug('grouped questions, below top level');
            $log.debug(groupedQuestions);

            /*jshint -W083 */
            //https://jslinterrors.com/dont-make-functions-within-a-loop
            var keys = Object.keys(groupedQuestions).sort();
            _.forEach(keys, function(parentId) {
                var questions = groupedQuestions[parentId];

                var orderedGroupedQuestions = _.sortBy(questions, function(q) { return q.orderId; });

                //in all ordered questions where it has an answer.selectedId of parentId
                var parentInOrderedList = _.find(orderedQuestions, function(oq) {

                    var answer = _.find(oq.answers, function(a) {
                        return a.selectedId === parentId;
                    });

                    return answer;
                });

                //A parent may actually come after a child. Just proceed on and catch it in a future round
                if (parentInOrderedList) {
                    //Insert after the parent and then insert after last added
                    var idxToLookFor = parentInOrderedList.id;

                    _.forEach(orderedGroupedQuestions, function(ogq) {
                        var idx = _.findIndex(orderedQuestions, { 'id': idxToLookFor });
                        orderedQuestions.splice(idx + 1, 0, ogq);
                        idxToLookFor = ogq.id;
                    });

                    //Clean out the ordered questions from the unOrderedList
                    _.forEach(orderedGroupedQuestions, function(oq) {
                        var idx = _.findIndex(unOrderedQuestions, { 'id': oq.id });

                        if (idx > -1) {
                            unOrderedQuestions.splice(idx, 1);
                        } else {
                            throw 'Cannot remove question from array ' + oq.id;
                        }
                    });
                }
            });

            //No questions were matched to a parent in this round, and they won't be in any future round.
            if (checkSum === unOrderedQuestions.length)
            {
                var orphanedQuestions = _.map(unOrderedQuestions, 'id');
                throw 'Issues sorting interview. No parents found for the following questions ' + orphanedQuestions.join();
            }
        }

        return orderedQuestions;
    };
}]);