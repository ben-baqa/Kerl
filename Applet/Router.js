class Router{

    static id = String(Math.floor(Math.random()*1000000))

    constructor() {

        // UI Identifier
        this.props = {
            id: String(Math.floor(Math.random()*1000000)),
            userLimit: 4,
            userMap: new Map()
        }

        

        // Port Definition
        this.ports = {
            default: {
                input: {type: undefined},
                output: {type: null},
                onUpdate: (user) => {
                    let i = this.props.userMap.get(user.id)
                    console.log(i, user)
                    if (i) this.update(i, user)
                },
            }
        }

        for (let i = 0; i < this.props.userLimit; i++){
            this.ports[i] = {
                input: {type: undefined},
                output: {type: undefined},
            }
        }
    }

    init = () =>  {
    }

    deinit = () => {}

    _userAdded = (user) => {
        if (this.props.userMap.size < this.props.userLimit) this.props.userMap.set(user.id, this.props.userMap.size)
    }

    _userRemoved = (user) => {
        this.props.userMap.delete(user.id)
    }
}
export {Router}