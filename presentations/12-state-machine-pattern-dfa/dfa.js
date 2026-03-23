/* jshint strict: true, undef: true, unused: false */

/*
  DFA to search for [men] or [women] in a block of text
  Jeff Adkisson & J. Brenton Phillips
  KSU CS6040, Theory of Computation, Dr. Jung, Spring 2016

  Built with:
  * html
  * css
  * javascript
  * bootstrap: https://getbootstrap.com/ [see css/bootstrap.css for license]
  * vis.js: http://visjs.org/ [see visjs/vis.js for license]

  Requires:
  * HTML5 web browser
*/

//generalized DFA mechanism that accepts a set of states
//and a set of transitions, then calls various events as each
//letter in the string is processed through the state machine

var dfa = (function() {
  "use strict";
  var prvate = {
    //default state set - will be updated by caller
    states: {
      one: 0 //state labels start with one, but are 0-based for indexing,
        //one is always the start state and must be defined as such
    }
  };

  //default transition set - will be updated by caller
  prvate.transitions = [{ //state 1
    label: 'one',
    transitions: [{
      letter: null,
      transitionTo: prvate.states.one
    }],
    otherwise: prvate.states.one,
    accepted: true
  }];

  //uses current state and next letter to find next state
  var getNextState = function(currentState, letter) {
    for (var i = 0; i < currentState.transitions.length; i++) {
      if (letter === currentState.transitions[i].letter) {
        return prvate.transitions[currentState.transitions[i].transitionTo];
      }
    }
    //not found, return otherwise
    return prvate.transitions[currentState.otherwise];
  };

  // Event Definitions
  // Will be updated by caller to handle specific functionality.
  // Default action for each event is to do nothing.
  // onStart - called at start of DFA execution, no data processed at this
  //           point, passes inbound string to function
  // onIndexChange - called at every letter change, passes index, current
  //                  letter, prior state and current state to function
  // onStateChange - called anytime state is changed, passes index, current
  //                 letter, prior state, and current state to function
  // onAccept - called anytime acceptance occurs. Passes index, current letter,
  //            and current state to functionality
  // onCompleted - called after final letter is processed. Passes final state
  //               to function

  var events = {
    onStart: function() {},
    onStateChange: function() {},
    onIndexChange: function() {},
    onAccept: function() {},
    onComplete: function() {}
  };

  //called at start of DFA execution, no data processed at this
  //point, passes inbound string to function
  var onStart = function(str, stepping) {
    if (typeof(events.onStart) === "function") {
      events.onStart(str, stepping);
    }
  };

  //called anytime state is change, passes index, current
  //letter, prior state, and current state to function
  var onStateChange = function(idx, letter, priorState, currentState) {
    if (typeof(events.onStateChange) === "function") {
      events.onStateChange(idx, letter, priorState, currentState);
    }
  };

  //called at every letter change, passes index, current
  //letter, prior state and current state to function
  var onIndexChange = function(idx, letter, priorState, currentState) {
    if (typeof(events.onIndexChange) === "function") {
      events.onIndexChange(idx, letter, priorState, currentState);
    }
  };

  //called anytime acceptance occurs. Passes index, current letter,
  //and current state to functionality
  var onAccept = function(idx, letter, currentState) {
    if (typeof(events.onAccept) === "function") {
      events.onAccept(idx, letter, currentState);
    }
  };

  //called after final letter is processed. Passes final state
  //to function
  var onComplete = function(finalState, stepping) {
    if (typeof(events.onComplete) === "function") {
      events.onComplete(finalState, stepping);
    }
  };

  //accepts the text to be searched, the state set, and the transition set
  //and starts processing at index 0. At each step, the appropriate
  //events are called. If states or transitions are not provided,
  //the default set is used.
  var executeDfa = function(text, states, transitions, stepTo, forceState) {

    //determine whether single stepping is occurring
    var stepping = typeof(stepTo) === "number";
    var stepFrom = stepping ? stepTo : 0;
    stepTo = stepping ? stepTo + 1 : text.length;
    stepTo = stepTo !== 0 ? stepTo : 1;

    //if states were passed in, replace default state set
    if (typeof(states) === "object") {
      prvate.states = states;
    }

    //if transition set was passed in, replace default transition set
    if (Array.isArray(transitions)) {
      prvate.transitions = transitions;
    }

    //raise start event, including whether single-stepping is enabled
    onStart(text, stepping);

    //initialize currentState with either the default state
    //or the state passed in by the single-step caller
    var currentState = forceState || transitions[prvate.states.one];

    //loop through all symbols in the string
    for (var idx = stepFrom; idx < stepTo; idx++) {
      var priorState = currentState;
      var letter = text[idx] === undefined || text[idx] === null
        ? "\u03B5" //epsilon
        : text[idx].toLowerCase();
      currentState = getNextState(currentState, letter);

      if (currentState !== priorState) {
        //state has changed... raise state change event
        onStateChange(idx, letter, priorState, currentState);
      }

      //index changes on every pass, so call it every time
      onIndexChange(idx, letter, priorState, currentState);

      //if current state is accepted, call acceptance event
      if (currentState.accepted) {
        onAccept(idx, letter, currentState);
      }
    }

    //processing complete, raise complete event
    onComplete(currentState, stepping);

    //return final state
    return currentState;
  };

  // REVEAL PUBLIC FUNCTIONS AND OJBECTS
  return {
    find: executeDfa,
    events: events
  };
})();
