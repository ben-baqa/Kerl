
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

    intro: {
      title: false,
      mode: 'multi',
      login: false
    },


    // App Logic
    graph:
    {
      nodes: [
        {name:'eeg', class: brainsatplay.plugins.biosignals.EEG},
        {name:'neurofeedback', class: brainsatplay.plugins.algorithms.Neurofeedback, params: {metric: 'Focus'}},
        {name:'command', class: brainsatplay.plugins.controls.Event},
        {name:'Brainstorm', class: brainsatplay.plugins.networking.Brainstorm, params: {

          onUserConnected: (u) => {
            let parser = settings.graph.nodes.find(n => n.name === 'Router')
            parser.instance._userAdded(u)
          },
      
          onUserDisconnected: (u) => {
            let parser = settings.graph.nodes.find(n => n.name === 'Router')
            parser.instance._userRemoved(u)
          },

        }},
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
                    function: 'PlayerOneUpdate',
                    type: 'boolean'
                },

                {
                  object: 'GameApplication',
                  function: 'PlayerTwoUpdate',
                  type: 'boolean'
                },

              {
                object: 'GameApplication',
                function: 'PlayerThreeUpdate',
                type: 'boolean'
              },

              {
              object: 'GameApplication',
              function: 'PlayerFourUpdate',
              type: 'boolean'
              },

            ]
          }
        },
        {
          name:'ui', 
          class: brainsatplay.plugins.interfaces.UI
        }
    ],

      edges: [

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
            target: 'unity:PlayerOneUpdate',
          },
          {
            source: 'Router:1',
            target: 'unity:PlayerTwoUpdate',
          },
          {
            source: 'Router:2',
            target: 'unity:PlayerThreeUpdate',
          },
          {
            source: 'Router:3',
            target: 'unity:PlayerFourUpdate',
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