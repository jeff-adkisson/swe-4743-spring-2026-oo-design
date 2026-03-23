/* jshint strict: true, undef: true, unused: false */
/* globals dfa, visualizer */

var dfaInst = (function() {
  "use strict";

  //private members used for maintaining the state of the UI
  var prvate = {
    wait: 0,
    speed: 200,
    text: "",
    timeouts: [] //all deferred UI events are stored here
  };

  //define states. By convention, [one] is the Start state.
  var states = {
    one: 0,
    two: 1,
    three: 2,
    four: 3,
    five: 4,
    six: 5
  };

  //Array containing the transition set. Each state can have zero or more
  //transitions and also defines whether reaching that state means [accepted]
  //The array position [0] maps to the one:0 entry in the states object,
  //array position [1] maps to two:1, etc.
  var transitions = [{ //state 1
    state: states.one,
    label: 'one',
    transitions: [{
      letter: 'w',
      transitionTo: states.five
    }, {
      letter: 'm',
      transitionTo: states.two
    }],
    otherwise: states.one,
    accepted: false
  }, { //state 2
    state: states.two,
    label: 'two',
    transitions: [{
      letter: 'e',
      transitionTo: states.three
    }, {
      letter: 'w',
      transitionTo: states.five
    }],
    otherwise: states.one,
    accepted: false
  }, { //state 3
    state: states.three,
    label: 'three',
    transitions: [{
      letter: 'n',
      transitionTo: states.four
    }, {
      letter: 'm',
      transitionTo: states.two
    }, {
      letter: 'w',
      transitionTo: states.five
    }],
    otherwise: states.one,
    accepted: false
  }, { //state 4
    state: states.four,
    label: 'four',
    transitions: [{
      letter: 'm',
      transitionTo: states.two
    }, {
      letter: 'w',
      transitionTo: states.five
    }],
    otherwise: states.one,
    accepted: true
  }, { //state 5
    state: states.five,
    label: 'five',
    transitions: [{
      letter: 'o',
      transitionTo: states.six
    }, {
      letter: 'm',
      transitionTo: states.two
    }],
    otherwise: states.one,
    accepted: false
  }, { //state 6
    state: states.six,
    label: 'six',
    transitions: [{
      letter: 'm',
      transitionTo: states.two
    }, {
      letter: 'w',
      transitionTo: states.five
    }],
    otherwise: states.one,
    accepted: false
  }];

  //enables the UI controls if they were disabled during execution
  function enableControls() {
    $("#start-dfa,#step-dfa,#search-text,#executionSpeed").removeClass("disabled").prop("disabled", false);
  }

  //clears all timeouts to immediately stop deferred execution
  //then re-enables controls
  function stopExecution() {
    for (var i = 0; i < prvate.timeouts.length; i++) {
      clearTimeout(prvate.timeouts[i]);
    }
    prvate.timeouts = [];
    enableControls();
    $("#start-dfa, #stop-dfa").toggle();
  }

  //returns true if string is empty or whitespace
  function isEmptyOrSpaces(str) {
    return str === null || str.match(/^ *$/) !== null;
  }

  //renders a log entry, such as Accepted or a letter entry
  //in the log window
  var showLog = function(str) {
    prvate.timeouts.push(setTimeout(function() {
      $("#dfaOutput").prepend("<p>" + str + "</p>");
    }, prvate.wait));
  };

  //clears the log window
  var resetOutput = function() {
    $("#dfaTextBefore,#dfaTextLetter,#dfaTextAfter").html("&nbsp;");
    $("#dfaOutput").empty();
  };

  //gets the speed of the per-letter execution
  //if invalid, sets to default of 200
  //if single-stepping, sets speed to 0 for fastest
  //response time
  var getSpeed = function(stepping) {
    if (stepping) {
      prvate.speed = 0;
      return;
    }
    var spd = $("#executionSpeed").val();
    spd = $.isNumeric(spd) ? spd : 200;
    prvate.speed = Number(spd);
    $("#executionSpeed").val(spd);
  };

  //draws the side scrolling box to show the letters
  //that have been processed, the current letter,
  //and the letters remaining to be processed
  var showTextPosition = function(idx, letter) {
    var upcoming = 25;
    prvate.wait += prvate.speed;
    prvate.timeouts.push(setTimeout(function() {
      letter = isEmptyOrSpaces(letter) ? "&nbsp;" : letter;
      var after = prvate.text.substr(idx + 1);

      $("#dfaTextLetter").html(letter);
      $("#dfaTextAfter").html(after);
    }, prvate.wait));
  };

  //updates the graph by selecting the proper node and edge
  var updateGraph = function(priorState, currentState) {
    prvate.timeouts.push(setTimeout(function() {
      var edge = (priorState.state + 1) + "_" + (currentState.state + 1);
      visualizer.setSelection({
        nodes: [(currentState.state + 1)],
        edges: [edge]
      }, {
        unselectAll: true,
        highlightEdges: false
      });
    }, prvate.wait));
  }

  //event to handle start of execution
  //sets start node in visual graph and configures
  //single-stepping, if active
  dfa.events.onStart = function(str, stepping) {
    getSpeed(stepping);
    stepping = stepping || false;
    if (!stepping) {
      $("#start-dfa,#step-dfa,#search-text,#executionSpeed").addClass("disabled").prop("disabled", true);
    }
    prvate.text = str;
    prvate.wait = 0;
    updateGraph({state:0}, {state:0});
  };

  //event to handle state change rendering
  //finds the selected node in the graph and highlights it
  dfa.events.onStateChange = function(idx, letter, priorState, currentState) {
      updateGraph(priorState, currentState);
  };

  //event to handle rendering movement along the string
  //shows the current letter in both the side-scroller
  //and the rolling log
  dfa.events.onIndexChange = function(idx, letter, priorState, currentState) {
    if (priorState.state === currentState.state) {
      updateGraph(priorState, currentState);
    }
    letter = isEmptyOrSpaces(letter) ? "&nbsp;" : letter;
    showTextPosition(idx, letter);
    showLog(('000' + idx).slice(-3) + ": " +
      "<span class='letter'> " + letter + "</span>, <strong>" +
      priorState.state + "</strong> to <strong>" + currentState.state +
      "</strong>");
  };

  //event to show when the DFA has accepted (found) a text string
  //of men or women
  dfa.events.onAccept = function(idx, letter, currentState) {
    showLog(('000' + idx).slice(-3) + ": " +
      "<span class='accepted'>Accepted!</span>");
  };

  //event to stop execution and return controls to input state
  //when entire string has been processed
  dfa.events.onComplete = function(finalState, stepping) {
    if (!stepping) {
      prvate.timeouts.push(setTimeout(function() {
        stopExecution();
      }, prvate.wait));
    }
  };

  //function to bind the UI controls to their click and change
  //events when the UI is ready
  var bindUi = function() {

    //configure stepper object for single-stepping through text
    var configStepper = function() {
      return {
        step: -1,
        state: null
      };
    };
    var atStep = configStepper();

    //configure change event for text box to reset UI and stepper
    //if text changes
    $("#search-text").on("change", function() {
      console.log("RESET");
      resetOutput();
      atStep = configStepper();
    });

    //bind change event for executionSpeed text box
    $("#executionSpeed").on("change", getSpeed);

    //bind start execution button
    $("#start-dfa").click(function() {
      var txt = $("#search-text").val().trim();
      resetOutput();
      dfa.find(txt, dfaInst.states, dfaInst.transitions);
      atStep = configStepper();
      $("#start-dfa, #stop-dfa").toggle();
    });

    //bind stop execution button
    $("#stop-dfa").click(function() {
      stopExecution();
    });

    //bind single-stepper button
    $("#step-dfa").click(function() {
      var txt = $("#search-text").val().trim();
      if (atStep.step === txt.length - 1 || txt === "") {
        atStep = configStepper();
      }
      if (atStep.step === -1) {
        resetOutput();
      }
      atStep.step += 1;
      atStep.state = dfa.find(txt, dfaInst.states, dfaInst.transitions, atStep.step, atStep.state);
    });

    //set controls to visible and ready to go
    enableControls();
  };

  //return public objects and methods
  return {
    states: states,
    transitions: transitions,
    bindUi: bindUi
  };

})();

//when jquery document ready event fires, bind the UI controls
$(function() {
  "use strict";
  dfaInst.bindUi();
});
