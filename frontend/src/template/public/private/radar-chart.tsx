import React from "react";
import {ITimeLine, IRadarChart, IRadios} from "../../../rev-send/get-time-line";
import {RadarChart, PolarGrid, PolarAngleAxis, PolarRadiusAxis, Radar, Legend, Text,  } from "recharts";

interface IPropsSIRadarChart {
    timeLine: IRadarChart,
    height: number,
}

interface IStateSIRadarChart {
}

export class SIRadarChart extends React.Component<IPropsSIRadarChart, IStateSIRadarChart> {
    constructor(props: IPropsSIRadarChart) {
        super(props);

        this.state = {}
    }

    private arrayMax(radios: IRadios[]): number {
        if (radios.length === 0) return 0;
        var count = radios[0].value;
        for (let i = 1; i < radios.length; i++) {
            if (count < radios[i].value) count = radios[i].value;
        }
        return count;
    }

    render() {
        const size = 130;
        console.log("aaa", this.arrayMax(this.props.timeLine.radios));
        return (
            <div style={{width: this.props.height +130, height: this.props.height}}>
                <RadarChart outerRadius={size}
                            width={this.props.height}
                            height={this.props.height} data={this.props.timeLine.radios} style={{color: "white"}}>

                    <defs>
                        <radialGradient id={"color2"}>
                            <stop offset={"0%"} stopColor={"#2451B7"} stopOpacity={.01}/>
                            <stop offset={"20%"} stopColor={"#2451B7"} stopOpacity={.01}/>
                            <stop offset={"90%"} stopColor={"#2451B7"} stopOpacity={.2}/>
                            <stop offset={"100%"} stopColor={"#2451B7"} stopOpacity={.2}/>
                        </radialGradient>
                    </defs>

                    <PolarGrid/>
                    <PolarAngleAxis dataKey={"vector"} tick={{fill: "white"}} tickSize={20}/>
                    <PolarRadiusAxis angle={90} domain={[0, this.arrayMax(this.props.timeLine.radios) + 2]}/>
                    <Radar name={"Wind Direction"} dataKey={"value"} stroke={"#2451B7"} fill={`url(#color2)`}/>
                    <Legend style={{color: "white"}}/>
                </RadarChart>
            </div>
        );
    }
}