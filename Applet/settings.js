
// Link to Project Assets
import {config} from './Build/buildconfig.js'
import {Unity} from './Unity.js'
import {Router} from './Router.js'

export const settings = {
    name: "Kerl",
    devices: ["EEG", "HEG"],
    author: "Juris Zebneckis",
    description: "",
    categories: ["WIP"],
    instructions:"",

    // App Logic
    graph:
    {
      nodes: [
        {name:'eeg', class: brainsatplay.plugins.biosignals.EEG},
        {name:'neurofeedback', class: brainsatplay.plugins.algorithms.Neurofeedback, params: {metric: 'Focus'}},
        {name:'command', class: brainsatplay.plugins.controls.Event},
        {name:'Brainstorm', class: brainsatplay.plugins.networking.Brainstorm},
        {name:'Router', class: Router},
        {
          name:'unity', 
          class: Unity, 
          // class: brainsatplay.plugins.utilities.Unity, // still need to fix routing for built-in plugin
          params:{
              config,
              onUnityEvent: (ev) => {
                // Parse Messages from Unity
                if (typeof ev === 'string'){
                  console.log('MESSAGE: ' + ev)
                }
              },
              commands: 
              [
                {
                    object: 'GameApplication',
                    function: 'UpdateCoherence',
                    type: 'number'
                },
                {
                    object: 'GameApplication',
                    function: 'UpdateFocus',
                    type: 'number'
                },
                {
                    object: 'GameApplication',
                    function: 'UpdateBlink',
                    type: 'boolean'
                }
            ]
          }
        },
        {
          name:'ui', 
          class: brainsatplay.plugins.interfaces.UI
        }
    ],

      edges: [

        // BRAIN
        {
          source: 'eeg:atlas',
          target: 'neurofeedback',
        },
        {
          source: 'neurofeedback',
          target: 'unity:UpdateFocus',
        },

          {
            source: 'command',
            target: 'unity:UpdateBlink',
          },
          
          {
            source: 'command',
            target: 'Brainstorm',
          },
          
          {
            source: 'Brainstorm:command',
            target: 'Router',
          },

          // Routes
          {
            source: 'Router:0',
            target: 'Unity:PlayerOneUpdate',
          },
          {
            source: 'Router:1',
            target: 'Unity:PlayerTwoUpdate',
          },
          {
            source: 'Router:2',
            target: 'Unity:PlayerThreeUpdate',
          },
          {
            source: 'Router:3',
            target: 'Unity:PlayerFourUpdate',
          },

          // Interface
          {
            source: 'unity:element',
            target: 'ui:content',
          }
      ]
    },
    connect: true
}