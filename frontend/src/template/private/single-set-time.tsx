import React from "react";
import {IStateTSMain} from "../template-main";
import "../../CSSBase.css"

interface IPropsSISetTime {
    main: IStateTSMain
}

interface IStateSISetTime {
}

export class SISetTime extends React.Component<IPropsSISetTime, IStateSISetTime> {
    constructor(props: IPropsSISetTime) {
        super(props);

        this.state = {}
    }

    private divTop() : JSX.Element {
        return (
            <div style={{color: "white"}}>
                Set Time
            </div>
        );
    }

    private divStartTime() : JSX.Element {
        return (
            <div style={{color: "white"}}>
                Set Start time or Now
            </div>
        );
    }

    private divEndTime() : JSX.Element {
        return (
            <div style={{color: "white"}}>
                Set End time
            </div>
        );
    }

    render() {
        return (
            <div className={"CSSBase-ColorBgMidDark"} style={{width: 400, height: "300px", marginLeft: "20%",
                display: "grid", gridTemplateColumns: "30px auto 30px",
                gridTemplateRows: "20px 40px 120px 120px 20px"
            }}>
                <div style={{gridRowStart: "2", gridColumnStart: "2", justifySelf: "center"}}>
                    { this.divTop() }
                </div>
                <div style={{gridRowStart: "3", gridColumnStart: "2", justifySelf: "center"}}>
                    { this.divStartTime() }
                </div>
                <div style={{gridRowStart: "4", gridColumnStart: "2", justifySelf: "center"}}>
                    { this.divEndTime() }
                </div>
            </div>
        );
    }
}