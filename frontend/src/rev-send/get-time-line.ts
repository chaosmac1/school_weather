import "./fetchNull";
import fetchNull, { fetchNullJson }from "./fetchNull";

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

export async function getTimeLineAll(props: {startTime: Date, endTime: Date, timeValue: string, timezoneOffset: number}): Promise<ITimeLineAll> {
    const sendString = "https://localhost:5002/ConTimeLine/all" + setParas(props);
    const res = await fetchNullJson<ITimeLineAll>(sendString);

    if (res === null || res === undefined) {
        return {
            temp: { points: [] },
            humidity: { points: [] },
            windDirection: { radios: [] },
            windSpeed: { points: [] }
        }
    }

    return res;
}

export async function getTemp(props: {startTime: Date, endTime: Date, timeValue: string, timezoneOffset: number}): Promise<ITimeLine> {
    const sendString = "https://localhost:5002/ConTimeLine/temp"  + setParas(props);

    const res = await fetchNullJson<ITimeLine>(sendString);

    if (res === null) return { points: [] };

    return res;
}

export async function getWindSpeed(props: {startTime: Date, endTime: Date, timeValue: string, timezoneOffset: number}): Promise<ITimeLine> {
    const sendString = "https://localhost:5002/ConTimeLine/windspeed" + setParas(props);

    const res = await fetchNullJson<ITimeLine>(sendString);

    if (res === null) return { points: [] };

    return res;
}

export async function getWind(props: {startTime: Date, endTime: Date, timeValue: string, timezoneOffset: number}): Promise<IRadarChart> {
    const sendString = "https://localhost:5002/ConTimeLine/windDirection" + setParas(props);
    const res = await fetchNullJson<IRadarChart>(sendString);

    if (res === null || res === undefined) return { radios: [] };

    return res;
}

export async function getHumidity(props: {startTime: Date, endTime: Date, timeValue: string, timezoneOffset: number}): Promise<ITimeLine> {
    const sendString = "https://localhost:5002/ConTimeLine/humidity" + setParas(props);
    const res = await fetchNullJson<ITimeLine>(sendString);

    if (res === null) return { points: [] };

    return res;
}

function setParas(paras: {startTime: Date, endTime: Date, timeValue: string, timezoneOffset: number}): string {
    return "?startTime=" + dateToISOString(paras.startTime) +
        "&endTime=" + dateToISOString(paras.endTime) +
        "&timeValue=" + paras.timeValue +
        "&timezoneOffset=" + paras.timezoneOffset;
}

function dateToISOString(date: Date): string {
    function pad(number: number) {
        return number < 10 ? '0' + number : number.toString();
    }

    return date.getUTCFullYear() +
        pad(date.getMonth() +1) +
        pad(date.getDate()) + 'T' +
        pad(date.getHours()) + ':' +
        pad(date.getMinutes()) + ':' +
        pad(date.getSeconds()) + 'Z';
};
