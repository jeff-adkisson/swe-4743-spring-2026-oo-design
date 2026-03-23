/* jshint strict: true, undef: true, unused: false */
/* globals vis, dfaInst */

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

// object to handle rendering a DFA graph of the men/women search dfa
// requires vis.js open source library for rendering.
// see visjs/vis.js for license or visit http://visjs.org/

var visualizer = (function() {
  "use strict";

  // node array corresponds to the states in our DFA
  // the first node is not used and is drawn white on white
  // simply to allow a start arrow to point to State 1
  var nodes = new vis.DataSet([{
    color: {
      border: '#fff',
      background: '#fff',
      highlight: {
        border: '#fff',
        background: '#fff'
      },
    },
    shadow: {
      enabled: false
    },
    id: 0,
    label: "",
    level: 0,
    group: 0,
    borderWidthSelected: 10,
  }, {
    id: 1,
    label: 1,
    level: 1,
    group: 0,
    borderWidthSelected: 10,
    color: {
      highlight: {
        border: 'black',
        background: 'orange'
      },
    },
  }, {
    id: 2,
    label: 2,
    level: 2,
    group: 1,
    borderWidthSelected: 10,
    color: {
      highlight: {
        border: 'black',
        background: 'orange'
      },
    },
  }, {
    id: 3,
    label: 3,
    level: 3,
    group: 1,
    borderWidthSelected: 10,
    color: {
      highlight: {
        border: 'black',
        background: 'orange'
      },
    },
  }, {
    id: 4,
    label: 4,
    level: 4,
    group: 2,
    borderWidth: 5,
    borderWidthSelected: 10,
    color: {
      highlight: {
        border: 'black',
        background: 'orange'
      },
    },
  }, {
    id: 5,
    label: 5,
    level: 2,
    group: 1,
    borderWidthSelected: 10,
    color: {
      highlight: {
        border: 'black',
        background: 'orange'
      },
    },
  }, {
    id: 6,
    label: 6,
    level: 3,
    group: 1,
    borderWidthSelected: 10,
    color: {
      highlight: {
        border: 'black',
        background: 'orange'
      },
    },
  }]);

  // edge array specifies the lines between
  // each node and the labels on each transition state
  var edges = new vis.DataSet([{
    id:"0_1",
    from: 0, //start
    to: 1,
    label: "Start",
    font: {
      size: 30,
      align: 'top'
    },
    arrows: "to",
    color: {
        color: "#aaa",
        highlight: "#000"
    }
  }, {
    id:"1_1",
    from: 1, // loop from 1 to 1 for epsilon or sigma *
    to: 1,
    label: "\u03A3*                            ", //\u03B5 epsilon
    font: {
      size: 40,
      align: 'horizontal'
    },
    arrows: "to",
    color: {
        color: "#aaa",
        highlight: "#000"
    }
  }, {
    id:"1_2",
    from: 1, //from 1 to 2 for m
    to: 2,
    label: "m",
    font: {
      size: 40,
      align: 'bottom'
    },
    arrows: "to",
    color: {
        color: "#aaa",
        highlight: "#000"
    }
  }, {
    id:"2_3",
    from: 2, //from 2 to 3 for e
    to: 3,
    label: "e",
    font: {
      size: 40,
      align: 'top'
    },
    arrows: "to",
    color: {
        color: "#aaa",
        highlight: "#000"
    }
  }, {
    id:"2_5",
    from: 2, //from 2 to 5 for w
    to: 5,
    label: "w",
    font: {
      size: 40,
      align: 'bottom'
    },
    arrows: "to",
    color: {
        color: "#aaa",
        highlight: "#000"
    }
  }, {
    id:"3_1",
    from: 3, //from 3 to 1 for sigma *
    to: 1,
    font: {
      size: 40,
      align: 'bottom'
    },
    arrows: "to",
    color: {
        color: "#aaa",
        highlight: "#000"
    }
  }, {
    id:"2_1",
    from: 2, //from 2 to 1 for sigma *
    to: 1,
    font: {
      size: 40,
      align: 'bottom'
    },
    arrows: "to",
    color: {
        color: "#aaa",
        highlight: "#000"
    }
  }, {
    id:"4_1",
    from: 4, //from 4 to 1 for sigma *
    to: 1,
    font: {
      size: 40,
      align: 'bottom'
    },
    arrows: "to",
    color: {
        color: "#aaa",
        highlight: "#000"
    }
  }, {
    id:"5_1",
    from: 5, //from 5 to 1 for sigma *
    to: 1,
    font: {
      size: 40,
      align: 'bottom'
    },
    arrows: "to",
    color: {
        color: "#aaa",
        highlight: "#000"
    }
  }, {
    id:"6_1",
    from: 6, //from 6 to 1 for sigma *
    to: 1,
    font: {
      size: 40,
      align: 'bottom'
    },
    arrows: "to",
    color: {
        color: "#aaa",
        highlight: "#000"
    }
  }, {
    id:"3_4",
    from: 3, //from 3 to 4 for n, accepted
    to: 4,
    label: "n",
    font: {
      size: 40,
      align: 'bottom'
    },
    arrows: "to",
    color: {
        color: "#aaa",
        highlight: "#000"
    }
  }, {
    id:"3_2",
    from: 3, //from 3 to 2 for m
    to: 2,
    label: "m",
    font: {
      size: 40,
      align: 'bottom'
    },
    arrows: "to",
    color: {
        color: "#aaa",
        highlight: "#000"
    }
  }, {
    id:"3_5",
    from: 3, //from 3 to 5 for w
    to: 5,
    label: "w",
    font: {
      size: 40,
      align: 'bottom'
    },
    arrows: "to",
    color: {
        color: "#aaa",
        highlight: "#000"
    }
  }, {
    id:"4_2",
    from: 4, //from 4 to 2 for m
    to: 2,
    label: "m",
    font: {
      size: 40,
      align: 'bottom'
    },
    arrows: "to",
    color: {
        color: "#aaa",
        highlight: "#000"
    }
  }, {
    id:"4_5",
    from: 4, //from 3 to 4 for n, accepted
    to: 5,
    label: "w",
    font: {
      size: 40,
      align: 'bottom'
    },
    arrows: "to",
    color: {
        color: "#aaa",
        highlight: "#000"
    }
  }, {
    id:"1_5",
    from: 1, //from 1 to 5 for w
    to: 5,
    label: "w",
    font: {
      size: 40,
      align: 'bottom'
    },
    arrows: "to",
    color: {
        color: "#aaa",
        highlight: "#000"
    }
  }, {
    id:"5_6",
    from: 5, //from 5 to 6 for o
    to: 6,
    label: "o",
    font: {
      size: 40,
      align: 'bottom'
    },
    arrows: "to",
    color: {
        color: "#aaa",
        highlight: "#000"
    }
  }, {
    id:"5_2",
    from: 5, //from 3 to 4 for n, accepted
    to: 2,
    label: "m",
    font: {
      size: 40,
      align: 'bottom'
    },
    arrows: "to",
    color: {
        color: "#aaa",
        highlight: "#000"
    }
  }, {
    id:"6_2",
    from: 6, //from 6 to 2 for m following o
    to: 2,
    label: "m",
    font: {
      size: 40,
      align: 'bottom'
    },
    arrows: "to",
    color: {
        color: "#aaa",
        highlight: "#000"
    }
  }, {
    id:"6_5",
    from: 6, //from 3 to 4 for n, accepted
    to: 5,
    label: "w",
    font: {
      size: 40,
      align: 'bottom'
    },
    arrows: "to",
    color: {
        color: "#aaa",
        highlight: "#000"
    }
  }]);

  // rendering options
  // hierarchical left/right rendering was used as that is
  // most common for DFAs
  var options = {
    interaction: {
      selectable: false,
      dragNodes: false
    },
    physics: {
      enabled: true,
      hierarchicalRepulsion: {
        centralGravity: 0.0,
        springLength: 100,
        springConstant: 0.01,
        nodeDistance: 270,
        damping: 0.09
      },
    },
    layout: {
      hierarchical: {
        enabled: true,
        direction: "LR",
        levelSeparation: 275
      }
    },
    nodes: {
      font: {
        size: 30
      },
      borderWidth: 2,
      shadow: true
    },
    edges: {
      width: 2,
      shadow: false,
      selfReferenceSize:80,
      selectionWidth: 5,
      smooth: {
        type: "curvedCCW",
        roundness: -0.25
      }
    },
    clickToUse: true
  };

  // the DFA graph will be drawn in the #dfaGraph element
  var container = document.getElementById('dfaGraph');

  // the data used by the DFA graph
  var data = {
    nodes: nodes,
    edges: edges
  };

  // render the DFA graph and return reference to caller
  return new vis.Network(container, data, options);
})();
