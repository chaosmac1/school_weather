import React, {ChangeEvent, CSSProperties} from "react";
import {IHistory} from "../i-history";
import {TimeLine} from "./public/private/time-line";
import {ITimeLine, IRadarChart, getTimeLineAll} from "../rev-send/get-time-line";
import {SIRadarChart} from "./public/private/radar-chart";
import imgSelectData from "./private/select.gif";
import {SICheckbox} from "../single/single-Checkbox";
import "../CSSBase.css"

enum ELineName {
    temp,
    windSpeed,
    humidity,
    windDirection
}

export interface IPropsTSMain { history: IHistory }
export interface IStateTSMain {
    key: string | null,
    manuel: boolean,
    startDate: Date,
    endDate: Date,
    diff: string
}

export class TSMain extends React.Component<IPropsTSMain, IStateTSMain> {
    private values: { humidity: ITimeLine, windDirection: IRadarChart, windSpeed: ITimeLine, temp: ITimeLine } | null;
    private setStart: { day: number, month: number, year: number };
    private setEnd: { day: number, month: number, year: number };
    private manuel: boolean;

    constructor(props:IPropsTSMain) {
        super(props);

        this.values = null;

        const now = new Date();

        this.setStart = { day: now.getDay() -1, month: now.getMonth(), year: now.getFullYear() };
        this.setEnd = { day: now.getDay(), month: now.getMonth(), year: now.getFullYear() };
        this.manuel = false;

        this.state = {
            key: null,
            manuel: false,
            startDate: new Date(new Date().getTime() - (24*60*60*1000)),
            endDate: new Date(),
            diff: "oneMin"
        };
    }

    componentDidMount() { this.updateMain(); }

    private async updateMain() {
        const setStart = new Date(this.setStart.year, this.setStart.month -1, this.setStart.day);
        const setEnd = !this.manuel? new Date(): new Date(this.setEnd.year, this.setEnd.month -1, this.setEnd.day);

        const fromDb = await getTimeLineAll({
            startTime: setStart,
            endTime: setEnd,
            timezoneOffset: new Date().getTimezoneOffset() / 60,
            timeValue: "fiveSek"
        })

        this.values = fromDb;
        this.forceUpdate();
    }

    private divSetTime() {
        const divMonth = (func : (e: ChangeEvent<HTMLSelectElement>) => void) => {
            return (<select onChange={func} style={{
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
            }}> {[
                <option value={"1"} key={"1"}> January </option>,
                <option value={"2"} key={"2"}> February </option>,
                <option value={"3"} key={"3"}> March </option>,
                <option value={"4"} key={"4"}> April </option>,
                <option value={"5"} key={"5"}> May </option>,
                <option value={"6"} key={"6"}> June </option>,
                <option value={"7"} key={"7"}> July </option>,
                <option value={"8"} key={"8"}> August </option>,
                <option value={"9"} key={"9"}> September </option>,
                <option value={"10"} key={"10"}> October </option>,
                <option value={"11"} key={"11"}> November </option>,
                <option value={"12"} key={"12"}> December </option>
            ]} </select>);
        };

        const dayDiv = (func: (e: ChangeEvent<HTMLSelectElement>) => void) => {
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
        };

        const divYear = (func: (e: ChangeEvent<HTMLSelectElement>) => void) => {
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
        };

        return (
            <div className={"CSSBase-ColorBgMidDark"} style={{width: 400, height: "300px", marginLeft: "20%",
                display: "grid", gridTemplateColumns: "30px auto 30px",
                gridTemplateRows: "20px 40px 120px 120px 20px"
            }}>
                <div style={{gridRowStart: "2", gridColumnStart: "2", justifySelf: "center"}}>
                    <div style={{color: "white"}}> Set Time </div>
                </div>
                <div style={{gridRowStart: "3", gridColumnStart: "2", justifySelf: "center"}}>
                    <div style={{color: "white"}}>
                        <div style={{textAlign: "center"}}> Set Start time or Now </div>
                        { dayDiv((e) => {
                            this.setStart.day = Number.parseInt(e.target.value)
                            this.updateMain();
                        }) }
                        { divMonth((e) => {
                            this.setStart.month = Number.parseInt(e.target.value)
                            this.updateMain();
                        }) }
                        { divYear((e) => {
                            this.setStart.year = Number.parseInt(e.target.value)
                            this.updateMain();
                        }) }
                    </div>
                </div>
                <div style={{gridRowStart: "4", gridColumnStart: "2", justifySelf: "center"}}>
                    <div style={{color: "white"}}>
                        <div style={{display: "grid", gridTemplateColumns: "120px 80px auto"}}>
                            <div style={{gridColumnStart: "1", justifySelf: "center", paddingTop: "4px"}}> Set End time </div>
                            <div style={{gridColumnStart: "3", alignSelf: "center"}}> <SICheckbox LambdaReturn={(e) => {
                                this.manuel = e;
                                console.log("state", this.manuel)
                                this.updateMain();
                            }} Text={"Manuel"}/> </div>

                        </div>

                        <div style={{}}>
                            {dayDiv((e) => {
                                this.setEnd.day = Number.parseInt(e.target.value)
                                this.updateMain();
                            })}
                            {divMonth((e) => {
                                this.setEnd.month = Number.parseInt(e.target.value)
                                this.updateMain();
                            })}
                            {divYear((e) => {
                                this.setEnd.year = Number.parseInt(e.target.value)
                                this.updateMain();
                            })}
                            {(() => {
                                if (!this.manuel) return(
                                    <div style={{position: "relative", minWidth: 300, minHeight: 50, marginTop: -40, opacity: 0.7}}
                                         className="CSSBase-ColorBgMidDark">
                                    </div>
                                )
                                return null;
                            })()}
                        </div>
                    </div>
                </div>
            </div>
        );
    }

    private divTimeline(width: string) : JSX.Element {
        return ( <div style={{minWidth: "calc(100vw - " + width + "px)"}}> {this.divSetTime()} </div>);
    }

    render() {
        if (this.values === null) return <div/>

        return lines({
            values: this.values,
            ele: {
                setting: this.divTimeline("400px"),
                infoOnOff: <div/>,
                key: this.state.key,
            }
        });
    }
}

interface ILines {
    values: { humidity: ITimeLine, windDirection: IRadarChart, windSpeed: ITimeLine, temp: ITimeLine },
    ele: {
        infoOnOff: JSX.Element,
        key: null | string,
        setting: JSX.Element
    }
}
function lines(props: ILines ) : JSX.Element {
    const divLineCart = (name: ELineName, timeline:ITimeLine) => {
        var titel: string;
        switch (name) {
            case ELineName.temp:
                titel = "Temperature"
                break;
            case ELineName.windSpeed:
                titel = "Wind Speed"
                break;
            case ELineName.humidity:
                titel = "Humidity"
                break;
            case ELineName.windDirection:
                titel = "Wind Direction"
                break;

        }
        return (<TimeLine timeLine={timeline} polar={false}
                          height={195} subWidth={0} name={titel}/>)
    };

    const divRadarChart = (name: ELineName, windDirection:IRadarChart, size: number) => {
        return(<SIRadarChart timeLine={windDirection} height={size}/>);
    }

    return(
        <div style={{
            display: "grid",
            gridTemplateColumns: "300px auto auto",
            gridTemplateRows: "2vh 98vh"
        }}>
            { props.ele.infoOnOff }
            <div style={{gridColumnStart: "1", gridColumnEnd: "4"}}>
                <div style={{display: "grid",
                    gridTemplateRows: "auto auto auto auto", gridTemplateColumns: "400px auto auto"}}>
                    <div style={{gridRow: "1", gridColumnStart: "1", gridColumnEnd: "2"}}>
                        {divRadarChart(ELineName.windDirection, props.values.windDirection, 400)}
                    </div>
                    <div style={{gridRow: "1", gridColumnStart: "2", gridColumnEnd: "4", justifySelf: "left", alignSelf: "center"}}>
                        {props.ele.setting}
                    </div>
                    <div style={{gridRow: "2", gridColumnStart: "1", gridColumnEnd: "4"}}>
                        {divLineCart(ELineName.humidity, props.values.humidity)}
                    </div>
                    <div style={{gridRow: "3", gridColumnStart: "1", gridColumnEnd: "4"}}>
                        {divLineCart(ELineName.windSpeed, props.values.windSpeed)}
                    </div>
                    <div style={{gridRow: "4", gridColumnStart: "1", gridColumnEnd: "4"}}>
                        {divLineCart(ELineName.temp, props.values.temp)}
                    </div>
                </div>
            </div>
        </div>
    );
}