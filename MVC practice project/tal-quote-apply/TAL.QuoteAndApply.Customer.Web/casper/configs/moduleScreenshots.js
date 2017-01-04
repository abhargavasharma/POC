// List of modules you want to take a screenshot of
var pages = [
  { 
    name: 'primary-button', 
    selector: '.tal-btn--primary', 
    url:'http://localhost:8000/#!/buttons'
  },
  { 
    name: 'tal-cover-product', 
    selector: 'tal-cover-product', 
    url:'http://localhost:8000/#!/components',
    shots:[
      { 
        setup : function(){
          casper.click('tal-cover-product .tal-form-switch');
        }
      }
    ]
  },
  { 
    name: 'styleguide-doc-block', 
    selector: '.styleguide-doc-block', 
    url:'http://localhost:8000/#!/components'
  }
];

module.exports = pages;

