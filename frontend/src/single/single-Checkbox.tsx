import React from "react";

import './single-Checkbox.css';

interface IStateSICheckbox { }
interface IPropsSICheckbox {
    LambdaReturn: (e: boolean) => void,
    Text:string,
}
export class SICheckbox extends React.Component<IPropsSICheckbox, IStateSICheckbox> {
    constructor(probs:IPropsSICheckbox) {
        super(probs);
    }

    render() {
        return (
            <div>
                <label className='SICheckbox-checkcontainer'>
                    <input type='checkbox' onClick={ (e) => {
                        // @ts-ignore
                        this.props.LambdaReturn(e.target.checked);
                    }}/>
                    <div className='SICheckbox-checkmark'/>
                    <div style={{paddingTop: "1px"}}>
                        {this.props.Text}
                    </div>
                </label>
            </div>
        );
    }
}
