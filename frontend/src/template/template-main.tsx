import React from "react";
import {IHistory} from "../i-history";
import {SITimeLines} from "./public/single-time-lines";
import {getTimeLineAll} from "../rev-send/get-time-line";
import {dateToTicks, TicksPerDay, TicksPerMinute, TicksPerMillisecond, TicksPerSecond, TicksPerHour} from "../ulitis/time";
import "../CSSBase.css"
import {SISetTime} from "./private/single-set-time";

export interface IPropsTSMain {
    history: IHistory
}
export interface IStateTSMain {
    key: string | null,
    now: boolean,
    startDate: number,
    endDate: number,
}
export class TSMain extends React.Component<IPropsTSMain, IStateTSMain> {
    constructor(props:IPropsTSMain) {
        super(props);

        this.state = { key: null, now: true, startDate: 0, endDate: 0 }
    }



    private divTimeline(width: string) : JSX.Element {
        return (<SITimeLines updateSpeedInMsg={5000} width={width}>
            <div style={{minWidth: "calc(100vw - " + width + "px)"}}>
                <SISetTime main={this.state}/>
            </div>
        </SITimeLines>);
    }

    private divHeader(width: string) : JSX.Element {
        return (
            <div style={{minWidth: "100%", minHeight: "100%"}}>
                asd
            </div>
        );
    }

    private divInfo(width: string) : JSX.Element {
        return (
            <div>
                fluf
            </div>
        );
    }

    render() {
        const infoOnOff = () => {
            if (this.state.key === null) return null;
            return(
                <div style={{background: "darkblue", gridColumnStart: "1", gridColumnEnd: "2", gridRowStart: "1", gridRowEnd: "3"}}>
                    {this.divInfo("300px")}
                </div>
            );
        }

        return (
            <div style={{
                display: "grid",
                gridTemplateColumns: "300px auto auto",
                gridTemplateRows: "2vh 98vh"
            }}>
                { infoOnOff() }
                <div style={{background: "red",
                    gridColumnStart: (() => { return this.state.key === null? "1": "2" })(), gridColumnEnd: "4"}}>
                    {this.divHeader(((() => { return this.state.key === null? "100%": "calc(100% - 300px)" })()))}
                </div>
                <div style={{
                    gridColumnStart: (() => { return this.state.key === null? "1": "2" })(), gridColumnEnd: "4"}}>
                    {this.divTimeline(((() => { return this.state.key === null? "100%": "calc(100% - 300px)" })()))}
                </div>
            </div>
        );
    }
}