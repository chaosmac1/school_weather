import React, {CSSProperties} from "react";
import {CartesianGrid, XAxis, YAxis, ResponsiveContainer, AreaChart, Area, Tooltip, Legend} from "recharts";
import {ITimeLine} from "../../../rev-send/get-time-line";

export interface data {
    x: number,
    y: number
}
interface IPropsTimeLine {
    polar: boolean,
    timeLine: ITimeLine,
    height: number,
    subWidth: number,
    name: string
}

interface IStateTimeLine {
}

export class TimeLine extends React.Component<IPropsTimeLine, IStateTimeLine> {
    constructor(props: IPropsTimeLine) {
        super(props);

        this.state = {}
    }

    render() {
        console.log("ooo", this.props.timeLine.points);
        return (
            <div style={{width:"calc(100% - 30px)", height: this.props.height}}>
                <ResponsiveContainer width={"100%"} height={this.props.height - 20}>
                    <AreaChart data={this.props.timeLine.points}>
                        <defs>
                            <linearGradient id="color" x1="0" y1="0" x2="0" y2="100%">
                                <stop offset={"0%"} stopColor={"#2451B7"} stopOpacity={.3}/>
                                <stop offset={"70%"} stopColor={"#2451B7"} stopOpacity={.23}/>
                                <stop offset={"90%"} stopColor={"#2451B7"} stopOpacity={.05}/>
                                <stop offset={"100%"} stopColor={"#2451B7"} stopOpacity={.05}/>
                            </linearGradient>
                        </defs>

                        <Area dataKey={"value"} stroke={"#2451B7"} fill={`url(#color)`} name={this.props.name}/>
                        <XAxis dataKey={"date"} minTickGap={35} attributeName={"halo"}/>
                        <YAxis dataKey={"value"} axisLine={false}/>

                        <Tooltip/>

                        <CartesianGrid opacity={0.2} vertical={false}/>
                        <Legend style={{color: "white"}}/>
                    </AreaChart>
                </ResponsiveContainer>
            </div>
        );
    }
}