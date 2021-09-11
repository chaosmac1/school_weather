import React from "react";
import {TimeLine} from "./private/time-line";
import {getTimeLineAll, IRadios, ITimeLine, ITimeLineAll, IRadarChart} from "../../rev-send/get-time-line";
import {dateToTicks, TicksPerHour} from "../../ulitis/time";
import {IPropsTSMain, IStateTSMain} from "../template-main";
import {SIRadarChart} from "./private/radar-chart";

enum ELineName {
    temp,
    windSpeed,
    humidity,
    windDirection
}

interface IXY {
    x: number,
    y: number
}
interface IPropsSITimeLines {
    width: string,
    updateSpeedInMsg: number,
    children: React.ReactNode
}

interface IStateSITimeLines {
    humidity: ITimeLine,
    windDirection: IRadarChart,
    windSpeed: ITimeLine,
    temp: ITimeLine,
}

export class SITimeLines extends React.Component<IPropsSITimeLines, IStateSITimeLines> {
    constructor(props: IPropsSITimeLines) {
        super(props);

        this.state = {
            humidity: { points: [] },
            temp: { points: [] },
            windDirection: { radios: [] },
            windSpeed: { points: [] }
        };
    }

    componentDidMount() { this.update() }

    private async update() {
        var timeDate = new Date();
        const fromDb = await getTimeLineAll(dateToTicks(timeDate), dateToTicks(timeDate) + (TicksPerHour * 2));
        this.setState({
            temp: fromDb.temp,
            windSpeed: fromDb.windSpeed,
            humidity: fromDb.humidity,
            windDirection: fromDb.windDirection
        });
    }


    private divLineCart(name: ELineName, timeline:ITimeLine) : JSX.Element {
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
    }

    private divRadarChart(name: ELineName, windDirection:IRadarChart, size: number) {
        return(<SIRadarChart timeLine={windDirection} height={size}/>);
    }

    render() {
        return (
            <div style={{width: this.props.width,display: "grid",
                gridTemplateRows: "auto auto auto auto", gridTemplateColumns: "400px auto auto"}}>
                <div style={{gridRow: "1", gridColumnStart: "1", gridColumnEnd: "2"}}>
                    {this.divRadarChart(ELineName.windDirection, this.state.windDirection, 400)}
                </div>
                <div style={{gridRow: "1", gridColumnStart: "2", gridColumnEnd: "4", justifySelf: "left", alignSelf: "center"}}>
                    {this.props.children}
                </div>
                <div style={{gridRow: "2", gridColumnStart: "1", gridColumnEnd: "4"}}>
                    {this.divLineCart(ELineName.humidity, this.state.humidity)}
                </div>
                <div style={{gridRow: "3", gridColumnStart: "1", gridColumnEnd: "4"}}>
                    {this.divLineCart(ELineName.windSpeed, this.state.windSpeed)}
                </div>
                <div style={{gridRow: "4", gridColumnStart: "1", gridColumnEnd: "4"}}>
                    {this.divLineCart(ELineName.temp, this.state.temp)}
                </div>
            </div>
        );
    }
}