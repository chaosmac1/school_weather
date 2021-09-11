export interface IPoint {
    pointId: number;
    value: number;
    date: string;
}

export interface ITimeLine {
    points: IPoint[]
}

export interface IRadarChart {
    radios: IRadios[]
}
export interface IRadios {
    vector: number,
    value: number
}
export interface ITimeLineAll {
    temp: ITimeLine,
    windSpeed: ITimeLine,
    humidity: ITimeLine,
    windDirection: IRadarChart,
}

export async function getTimeLineAll(startTimeTicks: number, endTimeTicks: number): Promise<ITimeLineAll> {
    return await (await fetch("https://localhost:5001/ConTimeLine/all?startTime=" +
        startTimeTicks + "&endTime=" +
        endTimeTicks + "&timeValue=test")).json();
}