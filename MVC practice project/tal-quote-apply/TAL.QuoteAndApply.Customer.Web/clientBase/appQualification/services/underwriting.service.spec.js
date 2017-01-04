'use strict';

describe('Service: underwritingService', function () {

    // load the service's module
    beforeEach(module('appQualification'));

    var underwritingService;

    beforeEach(inject(function (_talUnderwritingService_) {
        underwritingService = _talUnderwritingService_;
    }));

    it('clearAllAnswers should set all answers to unselected', function () {

        //Arrange
        var question = {
            answers: [
                { isSelected: false},
                { isSelected: true}
            ]
        };

        //Act
        underwritingService.clearAllAnswers(question);

        //Assert
        var selectedAnswer = _.find(question.answers, {isSelected: true});
        expect(selectedAnswer).toBe(undefined);
    });

    it('setAnswerById should set isSelected if answer id found', function () {

        //Arrange
        var question = {
            answers: [
                { id: 'a1', isSelected: false },
                { id: 'a2', isSelected: false }
            ]
        };

        //Act
        underwritingService.setAnsweredById(question, 'a2');

        //Assert
        var selectedAnswer = _.find(question.answers, {isSelected: true});
        expect(selectedAnswer).toNotBe(undefined);
        expect(selectedAnswer.id).toBe('a2');
        expect(selectedAnswer.isSelected).toBe(true);
    });

    it('setAnswerById should throw exception if answer id not found', function () {

        //Arrange
        var question = {
            answers: [
                { id: 'a1', isSelected: false },
                { id: 'a2', isSelected: false }
            ]
        };

        var functionCall = function() {
            underwritingService.setAnsweredById(question, 'a3');
        };

        //Act, Assert
        expect(functionCall).toThrow('answer not found');
    });

    it('getAllNonNoneAnswers should return all answers not marked with isNoneOption', function () {

        //Arrange
        var question = {
            answers: [
                { id: 'a1', isNoneOption: false },
                { id: 'a2', isNoneOption: true },
                { id: 'a3', isNoneOption: false }
            ]
        };

        //Act
        var nonNoneAnswers = underwritingService.getAllNonNoneAnswers(question);

        //Assert
        expect(nonNoneAnswers.length).toBe(2);
        expect(nonNoneAnswers[0].id).toBe('a1');
        expect(nonNoneAnswers[1].id).toBe('a3');
        var noneAnswer = _.find(nonNoneAnswers, {isNoneOption: true});
        expect(noneAnswer).toBe(undefined);
    });

    it('getSingleNoneAnswer should return one none answer', function () {

        //Arrange
        var question = {
            answers: [
                { id: 'a1', isNoneOption: false },
                { id: 'a2', isNoneOption: true },
                { id: 'a3', isNoneOption: false }
            ]
        };

        //Act
        var noneAnswer = underwritingService.getSingleNoneAnswer(question);

        //Assert
        expect(noneAnswer).toNotBe(null);
        expect(noneAnswer.id).toBe('a2');
    });

    it('getSingleNoneAnswer should be undefined if no none answer', function () {

        //Arrange
        var question = {
            answers: [
                { id: 'a1', isNoneOption: false },
                { id: 'a2', isNoneOption: false },
                { id: 'a3', isNoneOption: false }
            ]
        };

        var noneAnswer = underwritingService.getSingleNoneAnswer(question);

        //Act, Assert
        expect(noneAnswer).toBe(undefined);
    });

    it('toggleSelectedAnswer should toggle selected status of answer', function () {

        //Arrange
        var answer = { isSelected: false };

        //Toggle on
        underwritingService.toggleAnswerSelected(answer);
        expect(answer.isSelected).toBe(true);

        //Toggle off
        underwritingService.toggleAnswerSelected(answer);
        expect(answer.isSelected).toBe(false);
    });

    it('getSelectedSingleAnswer should return single selected answer', function () {

        //Arrange
        var question = {
            answers: [
                { id: 'a1', isSelected: false },
                { id: 'a2', isSelected: true },
                { id: 'a3', isSelected: false }
            ]
        };

        //Act
        var selectedAnswer = underwritingService.getSelectedSingleAnswer(question);

        //Assert
        expect(selectedAnswer).toNotBe(undefined);
        expect(selectedAnswer.id).toBe('a2');
    });

    it('getSelectedSingleAnswer should return undefined if no selected answer', function () {

        //Arrange
        var question = {
            answers: [
                { id: 'a1', isSelected: false },
                { id: 'a2', isSelected: false },
                { id: 'a3', isSelected: false }
            ]
        };

        //Act
        var selectedAnswer = underwritingService.getSelectedSingleAnswer(question);

        //Assert
        expect(selectedAnswer).toBe(undefined);
    });

    it('hasAnySelectedAnswers should return true if has selected answer', function () {

        //Arrange
        var question = {
            answers: [
                { id: 'a1', isSelected: false },
                { id: 'a2', isSelected: true },
                { id: 'a3', isSelected: false }
            ]
        };

        //Act
        var hasSelectedAnswer = underwritingService.hasAnySelectedAnswers(question);

        //Assert
        expect(hasSelectedAnswer).toBe(true);
    });

    it('hasAnySelectedAnswers should return false if no selected answer', function () {

        //Arrange
        var question = {
            answers: [
                { id: 'a1', isSelected: false },
                { id: 'a2', isSelected: false },
                { id: 'a3', isSelected: false }
            ]
        };

        //Act
        var hasSelectedAnswer = underwritingService.hasAnySelectedAnswers(question);

        //Assert
        expect(hasSelectedAnswer).toBe(false);
    });

    it('applyRemovedQuestions should remove question from all questions', function () {

        //Arrange
        var questions = [
            { id: 'q1' },
            { id: 'q2' },
            { id: 'q3' }
        ];

        var removedQuestionIds = ['q2'];

        //Act
        underwritingService.applyRemovedQuestions(questions, removedQuestionIds);

        //Assert
        expect(questions.length).toBe(2);
        expect(questions[0].id).toBe('q1');
        expect(questions[1].id).toBe('q3');
    });

    it('applyAddedQuestions should add question', function () {

        //Arrange
        var questions = [
            { id: 'q1', answers: [{selectedId:null}] },
            { id: 'q2', answers: [{selectedId:'q2?a1'}] },
            { id: 'q3', answers: [{selectedId:null}] }
        ];

        var addedQuestions = [
            { id: 'q2?a1?qa', parentId: 'q2?a1' }
        ];

        //Act
        underwritingService.applyAddedQuestions(questions, addedQuestions);

        //Assert
        expect(questions.length).toBe(4);
        expect(questions[0].id).toBe('q1');
        expect(questions[1].id).toBe('q2');
        expect(questions[2].id).toBe('q3');
        expect(questions[3].id).toBe('q2?a1?qa');
    });

    
    it('applyChangedQuestions should update attributes on questions and answers', function () {

        //Arrange
        var questions = [
            {
                id: 'q1',
                parentId: 'original parent',
                text:'original text',
                answers: [
                    {id:'a1', isSelected:false, selectedId:''}]
            }
        ];

        var changedQuestions = [
            {
                id: 'q1',
                changedAttributes: { parentId: 'changed parent', text: 'changed text' },
                changedAnswers:[
                    {id:'a1', changedAttributes:{isSelected:true, selectedId:'qa?a1'}}]
            }
        ];

        //Act
        underwritingService.applyChangedQuestionUpdates(questions, changedQuestions);

        //Assert
        expect(questions.length).toBe(1);

        //TODO: add checks for other attributes that we update
        expect(questions[0].answers[0].isSelected).toBe(true);
        expect(questions[0].answers[0].selectedId).toBe('qa?a1');
    });

});