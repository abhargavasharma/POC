# Casper test

## TODO

- Allow to change the base URL so the backend developers can run the test from windows.

## CSS regression testing

### Command lines

To use that feature, go to the terminal and type:

```
    casperjs test casper/checkDocumentation.js
```

### Description

We are using image comparison to detect CSS related regression.

If you use the documentation component in the styleguide, it will automatically be handled by the css regression tool.

The documentation will be captured in all breakpoints and compared to the previous version.

*NOTE*: The test itself can take up to 15 minutes if you decide to cover the whole styleguide.

## Lighter css regression testing

### Command lines

```
    casperjs test casper/component.js
```

### Description

This is a lighter version of the above test. It will baiscally do the CSS regression testing on the module defined, as opposed to the whole styleguide.

You may also add some steps to do before taking the screenshots.

## HTML testing

### Command lines

To run the test, go to the terminal and type:

To run the tests on the styleguide:

```
    casperjs test casper/integration.js
```

To run the tests on all projects:

```
    casperjs test --files casper/integration.js
```

### Description

We have a casper test to test all our component across the project.

Through the test script, we check every module we have tests for and validate those modules against a set of rule.

### Writing tests

on casper/modules/custom a test is written like this:

```
    {
		'cssSelector' : function(selector, pageName) {
			// test written here...
		}
    }

```

on casper/modules/accessibility a test is written the same way:

```
    {
		'cssSelector' : function(selector, pageName) {
			// test written here...
		}
    }

```


All the tests available are listed in this file:

casper/helpers/

 