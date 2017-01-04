'use strict';

angular.module('styleguide')
  .controller('QuestionsCtrl', ['$scope', function ($scope) {
    $scope.questions = [
      {
        questionType: 'DateOfBirth',
        text: 'Lorem ipsum dolor sit amet, consectetur adipisicing elit. At, adipisci.',
        id: 'sgquestion01',
        answers: [
          {
            value: 'Yes',
            text: 'Yes',
            tags: []
          },
          {
            value: 'No',
            text: 'No',
            tags: []
          }
        ]
      },
      {
        questionType: 'Currency',
        text: 'Lorem ipsum dolor sit amet, consectetur adipisicing elit. At, adipisci.',
        id: 'sgquestion02',
        answers: [
          {
            value: 'Yes',
            text: 'Yes',
            tags: []
          },
          {
            value: 'No',
            text: 'No',
            tags: []
          }
        ]
      },
      {
        questionType: 'SingleSelect',
        text: 'Lorem ipsum dolor sit amet, consectetur adipisicing elit. At, adipisci.',
        id: 'sgquestion03',
        answers: [
          {
            value: 'Yes',
            text: 'Yes',
            tags: []
          },
          {
            value: 'No',
            text: 'No',
            tags: []
          }
        ]
      },
      {
        questionType: 'SingleSelect',
        text: 'Lorem ipsum dolor sit amet, consectetur adipisicing elit. At, adipisci.',
        id: 'sgquestion031',
        answers: [
          { value: 'First answer', text: 'First answer', tags: [] },
          { value: 'Second answer', text: 'Second answer', tags: [] },
          { value: 'Third answer', text: 'Third answer', tags: [] },
          { value: 'Lorem ipsum dolor sit', text: 'Lorem ipsum dolor sit', tags: [] },
          { value: 'Lorem ipsum dolor sit amet, consectetur adipisicing elit. Natus.', text: 'Lorem ipsum dolor sit amet, consectetur adipisicing elit. Reprehenderit.', tags: [] },
          { value: 'Fourth answer', text: 'Fourth answer', tags: [] },
          { value: 'Fifth answer', text: 'Fifth answer', tags: [] },
          { value: 'Sixth answer', text: 'Sixth answer', tags: [] },
          { value: 'Seventh answer', text: 'Seventh answer', tags: [] }
        ]
      },
      {
        questionType: 'SingleSelectList',
        text: 'Lorem ipsum dolor sit amet, consectetur adipisicing elit. At, adipisci.',
        id: 'sgquestion04',
        answers: [
          { value: 'First answer', text: 'First answer', tags: [] },
          { value: 'Second answer', text: 'Second answer', tags: [] },
          { value: 'Third answer', text: 'Third answer', tags: [] },
          { value: 'Lorem ipsum dolor sit', text: 'Lorem ipsum dolor sit', tags: [] },
          { value: 'Lorem ipsum dolor sit amet, consectetur adipisicing elit. Natus.', text: 'Lorem ipsum dolor sit amet, consectetur adipisicing elit. Reprehenderit.', tags: [] },
          { value: 'Fourth answer', text: 'Fourth answer', tags: [] },
          { value: 'Fifth answer', text: 'Fifth answer', tags: [] },
          { value: 'Sixth answer', text: 'Sixth answer', tags: [] },
          { value: 'Seventh answer', text: 'Seventh answer', tags: [] }
        ]
      },
      {
        questionType: 'SingleSelectAutoComplete',
        text: 'Lorem ipsum dolor sit amet, consectetur adipisicing elit. At, adipisci.',
        id: 'sgquestion05',
        answers: [
          {
            value: 'Yes',
            text: 'Yes',
            tags: []
          },
          {
            value: 'No',
            text: 'No',
            tags: []
          }
        ]
      },
      {
        questionType: 'SingleSelectIcons',
        text: 'Lorem ipsum dolor sit amet, consectetur adipisicing elit. At, adipisci.',
        id: 'sgquestion06',
        answers: [
          {
            value: 'Yes',
            text: 'Yes',
            tags: []
          },
          {
            value: 'No',
            text: 'No',
            tags: []
          }
        ]
      },
      {
        questionType: 'MultiSelect',
        text: 'Lorem ipsum dolor sit amet, consectetur adipisicing elit. At, adipisci.',
        id: 'sgquestion07',
        answers: [
          {
            value: 'Answer 1',
            text: 'Answer 1',
            tags: [],
            helpText: 'Lorem ipsum dolor sit amet.'
          },
          {
            value: 'Answer 2',
            text: 'Answer 3',
            tags: []
          },
          {
            value: 'Answer 3',
            text: 'Answer 3',
            tags: [],
            helpText: 'Lorem ipsum dolor sit amet.'
          },
          {
            value: 'Answer 4',
            text: 'Answer 4',
            tags: [],
            helpText: 'Lorem ipsum dolor sit amet.'
          },
          {
            value: 'Answer 5',
            text: 'Answer 5',
            tags: [],
            helpText: 'Lorem ipsum dolor sit amet.'
          }
        ]
      },
      {
        questionType: 'SportIcons',
        text: 'Lorem ipsum dolor sit amet, consectetur adipisicing elit. At, adipisci.',
        id: 'sgquestion08',
        answers: [
          {
            value: 'Yes',
            text: 'Yes',
            tags: []
          },
          {
            value: 'No',
            text: 'No',
            tags: []
          }
        ]
      },
      {
        questionType: 'MultiSelectAutoComplete',
        text: 'Lorem ipsum dolor sit amet, consectetur adipisicing elit. At, adipisci.',
        id: 'sgquestion09',
        answers: [
          {
            value: 'Yes',
            text: 'Yes',
            tags: []
          },
          {
            value: 'No',
            text: 'No',
            tags: []
          }
        ]
      },
      {
        questionType: 'MultiSelectCheckBoxes',
        text: 'Lorem ipsum dolor sit amet, consectetur adipisicing elit. At, adipisci.',
        id: 'sgquestion10',
        answers: [
          {
            value: 'Yes',
            text: 'Yes',
            tags: []
          },
          {
            value: 'No',
            text: 'No',
            tags: []
          }
        ]
      },
      {
        questionType: 'Unsupported',
        id: 'sgquestion11',
        answers: [
          {
            value: 'Yes',
            text: 'Yes',
            tags: []
          },
          {
            value: 'No',
            text: 'No',
            tags: []
          }
        ]
      },
      {
        questionType: 'MultiSelectCheckBoxes',
        context: 'Lorem ipsum',
        text: 'Lorem ipsum dolor sit amet, consectetur adipisicing elit. At, adipisci.',
        id: 'sgquestion12',
        answers: [
          {
            value: 'Yes',
            text: 'Yes',
            tags: []
          },
          {
            value: 'No',
            text: 'No',
            tags: []
          }
        ]
      },
      {
        questionType: 'MultiSelectCheckBoxes',
        context: 'Lorem ipsum dolor sit amet, consectetur adipisicing elit. Cum non, accusantium ratione voluptatibus nostrum atque.',
        text: 'Lorem ipsum dolor sit amet, consectetur adipisicing elit. At, adipisci.',
        id: 'sgquestion12',
        answers: [
          {
            value: 'Yes',
            text: 'Yes',
            tags: []
          },
          {
            value: 'No',
            text: 'No',
            tags: []
          }
        ]
      }
    ];
  }]);