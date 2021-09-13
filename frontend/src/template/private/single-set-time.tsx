import React, {ChangeEvent, CSSProperties} from "react";
import {IStateTSMain} from "../template-main";
import "../../CSSBase.css"
import imgSelectData from "./select.gif";
import {SICheckbox} from "../../single/single-Checkbox";

interface IPropsSISetTime {
    main: IStateTSMain
}

interface IStateSISetTime {
    now: boolean
}

export class SISetTime extends React.Component<IPropsSISetTime, IStateSISetTime> {
    constructor(props: IPropsSISetTime) {
        super(props);

        this.state = {now: false}
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
                <div style={{textAlign: "center"}}> Set Start time or Now </div>
                {this.dayDiv((e) => {})}
                {this.divMonth((e) => {})}
                {this.divYear((e) => {})}
            </div>
        );
    }

    private divEndTime() : JSX.Element {
        return (
            <div style={{color: "white"}}>
                <div style={{display: "grid", gridTemplateColumns: "120px 80px auto"}}>
                    <div style={{gridColumnStart: "1", justifySelf: "center", paddingTop: "4px"}}> Set End time </div>
                    <div style={{gridColumnStart: "3", alignSelf: "center"}}> <SICheckbox LambdaReturn={(e) => {
                        this.setState({now: e})
                        console.log("state", this.state.now)
                    }} Text={"Manuel"}/> </div>

                </div>

                <div style={{}}>
                    {this.dayDiv((e) => {})}
                    {this.divMonth((e) => {})}
                    {this.divYear((e) => {})}
                    {(() => {
                            if (this.state.now) return(
                                <div style={{position: "relative", minWidth: 300, minHeight: 50, marginTop: -40, opacity: 0.7}}
                                     className="CSSBase-ColorBgMidDark">
                                </div>
                            )
                            return null;
                    })()}
                </div>
            </div>
        );
    }

    private divMonth(func : (e: ChangeEvent<HTMLSelectElement>) => void): JSX.Element {
        const monthStyle: CSSProperties = {
            color: "#ffffff",
            backgroundColor: "#0f1318",
            border: "1px #13181f solid",
            borderRadius: "7px",
            appearance: "none",
            backgroundRepeat: "no-repeat",
            backgroundPosition: "right",
            paddingTop: "8px",
            marginRight: "10px",
            minHeight: "35px",
            paddingLeft: "15px",
            backgroundImage: `url(${imgSelectData})`,
            minWidth: "140px"
        };
        let optionDivList = [];
        optionDivList.push(<option value={"1"} key={"1"}> January </option>);
        optionDivList.push(<option value={"2"} key={"2"}> February </option>);
        optionDivList.push(<option value={"3"} key={"3"}> March </option>);
        optionDivList.push(<option value={"4"} key={"4"}> April </option>);
        optionDivList.push(<option value={"5"} key={"5"}> May </option>);
        optionDivList.push(<option value={"6"} key={"6"}> June </option>);
        optionDivList.push(<option value={"7"} key={"7"}> July </option>);
        optionDivList.push(<option value={"8"} key={"8"}> August </option>);
        optionDivList.push(<option value={"9"} key={"9"}> September </option>);
        optionDivList.push(<option value={"10"} key={"10"}> October </option>);
        optionDivList.push(<option value={"11"} key={"11"}> November </option>);
        optionDivList.push(<option value={"12"} key={"12"}> December </option>);

        return (<select onChange={func} style={monthStyle}> {optionDivList} </select>);
    }

    private dayDiv(func: (e: ChangeEvent<HTMLSelectElement>) => void) {
        const dayStyle: CSSProperties = {
            color: "#ffffff",
            backgroundColor: "#0f1318",
            border: "1px #13181f solid",
            borderRadius: "7px",
            appearance: "none",
            backgroundRepeat: "no-repeat",
            backgroundPosition: "right",
            marginRight: "10px",
            minHeight: "35px",
            paddingLeft: "15px",
            backgroundImage: `url(${imgSelectData})`,
            minWidth: "53px",
            paddingTop: "8px",
            marginLeft: 10
        };
        let optionDivList = [];
        for (let i = 1; i < 32; i++)
            optionDivList.push(<option value={i.toString()} key={i.toString()}> {(i).toString()}</option>);

        return (
            <select onChange={func} style={dayStyle}> {optionDivList} </select>
        );
    }

    private divYear(func: (e: ChangeEvent<HTMLSelectElement>) => void) {
        const yearStyle: CSSProperties = {
            color: "#ffffff",
            backgroundColor: "#0f1318",
            border: "1px #13181f solid",
            borderRadius: "7px",
            appearance: "none",
            backgroundRepeat: "no-repeat",
            backgroundPosition: "right",
            marginRight: "10px",
            minHeight: "35px",
            paddingTop: "8px",
            paddingLeft: "15px",
            backgroundImage: `url(${imgSelectData})`,
            minWidth: "77px",
        };
        const year = (new Date()).getFullYear();
        let optionDivList = [];

        for (let i = year; i > 1999; i--) {
            optionDivList.push(<option value={i.toString()} key={i.toString()}> {(i).toString()} </option>);
        }

        return (
            <select onChange={func} style={yearStyle}> {optionDivList} </select>
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