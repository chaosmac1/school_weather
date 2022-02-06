import {address} from "./address";
import "./fetchWithTimeout";
import fetchWithTimeout from "./fetchWithTimeout";
import {fetchNullJson} from "./fetchNull";
import {ITimeLine} from "./get-time-line";

interface IJsonResCheckIfLoginRight {
    response: number,
    msg: string,
    data: {
        usernameRight: boolean,
        passwordRight: boolean
    }
}

export async function CheckIfLoginRight(props: {username: string, passwd: string}): Promise<boolean> {
    const json: IJsonResCheckIfLoginRight | null = await fetchNullJson<IJsonResCheckIfLoginRight>(address + "ConLogin/login", {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify({
            username: props.username,
            passwd: props.passwd
        }),
    });

    if (json === null) return false;

    return (json.response?? 0) !== 200 || json.data === null || json.data === undefined
        ? false
        : json.data.usernameRight && json.data.passwordRight;
}

interface IJsonResChangePasswd {
    response: number,
    msg: string,
    data: {
        usernameRight: boolean,
        passwordRight: boolean
    }
}

export async function ChangePasswd(props: { username: string, passwd: string, newPasswd: string }): Promise<boolean>  {
    let fetchRes: IJsonResChangePasswd | null = null;

    const res: IJsonResChangePasswd | null = await fetchNullJson<IJsonResChangePasswd>(address + "ConLogin/changepasswd", {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify({
            username: props.username,
            oldPasswd: props.passwd,
            newPasswd: props.newPasswd,
        }),
    });


    if (res === null) return false;

    return (res.response?? 0) !== 200 || res.data === null || res.data === undefined
        ? false
        : res.data.usernameRight && res.data.passwordRight;
}

interface IGetLogs {
    response: number,
    msg: string,
    data: {
        usernameRight: boolean,
        passwordRight: boolean,
        ips: {dateTime: number, ip: string}[]
    }
}
export async function GetLogs(props: {username: string, passwd: string}): Promise<{ip: string, date: Date}[]> {
    const res: IGetLogs | null = await fetchNullJson<IGetLogs>(address + "ConLogin/postlogs", {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify({
            username: props.username,
            passwd: props.passwd,
        }),
    });

    if (res === null) return [];

    if ((res.response?? 0) !== 200 || res.data === null || res.data === undefined) return [];

    if (!res.data.passwordRight || !res.data.usernameRight) return [];

    let resData: {ip: string, date: Date}[] = [];

    res.data.ips.forEach(e => resData.push(
        {ip: e.ip, date: new Date(e.dateTime)}));
    return resData;
}














