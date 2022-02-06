import React, {Component, CSSProperties} from 'react';
import PropTypes from 'prop-types';
import "./template-root.css"
import {CheckIfLoginRight, GetLogs} from "../rev-send/get-post-root";

function sleep(ms: number) {
    return new Promise(resolve => setTimeout(resolve, ms));
}

const height: string = "30px";

interface ITsRootState {
    open: boolean,
    isLoginIn: boolean,
    rightLoginValue: {
        username: string,
        password: string
    } | null
}
export class TsRoot extends Component<{}, ITsRootState> {
    private username: string = "";
    private password: string = "";
    private logs: {ip: string, date: Date}[] | null = null;
    private logsUpdateOn: boolean = false;
    constructor(props: {}) {
        super(props);

        this.state = {open: false, isLoginIn: false, rightLoginValue: null};
    }

    componentWillUnmount() { this.logs = null; }

    componentDidUpdate(prevProps: Readonly<{}>, prevState: Readonly<ITsRootState>, snapshot?: any) {
        console.log("uwu", this.state);
    }

    private buttonBlack(props: {text: string, roundStyle: string, funClick: () => void, colorStyle: string | undefined}) {
        const style: CSSProperties = {
            width: "fit-content",
            height: height,
            borderColor: "transparent",
            borderRadius: props.roundStyle,
            paddingLeft: "10px",
            paddingRight: "15px",
            color: "white"
        }

        return (
            <button className={"CSSBase-ColorBgMidDark"} style={style} onClick={props.funClick}>
                {props.text}
            </button>
        );
    }

    private buttonLoginDiv() {
        return this.buttonBlack({
            text:  !this.state.open? "Open Logs": "Close Logs",
            roundStyle: "0px 17px 17px 0px",
            funClick: () => { this.setState({open: !this.state.open}); },
            colorStyle: undefined})
    }

    private inputText(defaultValue: string, onChange: (text: string) => void, password: boolean = false) {
        return(
            <div className={"CSSBase-ColorHighlight"}>
                <div className={"ts-root-login-field-input-Text-Text"}>
                    {defaultValue}
                </div>
                <input className={"ts-root-login-field-input-Text-input"} type={!password? "Text": "password"}
                       alt={defaultValue}
                       onChange={(e) => {
                           onChange(e.target.value);
                       }}/>
            </div>
        );
    }

    private loginFieldDiv() {
        if (!this.state.open || this.state.isLoginIn) return null;

        const buttonLogin = () => {
            const loginButtonId = "loginbutton";

            const clickFunc = async () => {
                if (!await CheckIfLoginRight({ username: this.username?? "", passwd: this.password?? "" })) {
                    const ele = document.getElementById(loginButtonId);
                    if (ele === undefined || ele === null) return;
                    ele.style.animation = "animation-button-error-swiggle 200ms forwards ease";
                    ele.addEventListener('animationend', () => {
                        document.getElementById(loginButtonId)!.style.animation = "";
                    });
                    return;
                }

                this.setState({
                    isLoginIn: true,
                    rightLoginValue: {
                        username: this.username,
                        password: this.password
                    }
                });
            }
            return(
                <button id={loginButtonId} className={"ts-root-login-field-login-button"} onClick={(e) => {clickFunc()}}>
                    Login
                </button>
            );
        };

        return(
            <div className={"ts-root-login-field-div"}>
                <div className={"ts-root-login-field"}>
                    <div style={{padding: "25px"}}>
                        <div className={"CSSBase-TextBig"} style={{ textAlign: "center"}}>
                            Login
                        </div>

                        <div style={{marginTop: "max(20px, 2vh)", minWidth: "fit-content"}}>
                            {this.inputText("Username", (e) => {
                                this.username = e; })}
                            {this.inputText("Password", (e) => {
                                this.password = e; }, true)}

                            <div className={"ts-root-login-field-login-button-div"}>
                                <div style={{display: "flex", width: "100%", flexDirection: "row-reverse"}}>
                                    {buttonLogin()}
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        );
    }


    private LogFieldDiv(): JSX.Element | null {
        const Logs = () => {
            if (this.logs === null) {
                (async () => {
                    if (this.logsUpdateOn) return;
                    this.logsUpdateOn = true;

                    console.log("Start Log");

                    const returnNull = () => {
                        this.logsUpdateOn = false;
                        this.logs = null;
                        return;
                    }

                    if (this.state.rightLoginValue === null) {
                        console.log("Login Value Are Null");
                        return returnNull();
                    }

                    const values: { ip: string; date: Date }[] = await GetLogs({
                        passwd: this.state.rightLoginValue!.password,
                        username: this.state.rightLoginValue!.username
                    })

                    if (values.length === 0) {
                        console.log("Log Array Length: " + values.length);
                        return returnNull();
                    }

                    this.logsUpdateOn = false;
                    this.logs = values;
                    console.log("logs:", this.logs);
                    this.forceUpdate();
                    return;
                })();

                return(
                    <div className={"CSSBase-TextBig"} style={{
                        textAlign: "center",
                        marginTop: "max(40px, 2.9vh)"
                    }}>
                        Loading ...
                    </div>
                );
            }


            const divList: JSX.Element[] = []

            this.logs!.forEach((e, i) => {
                divList.push(
                    <div key={e.date.getTime().toString() + i} className={"ts-root-log-row"}>
                        {e.date.toString() + " : " + e.ip}
                    </div>
                )
            });
            return(
                <div>
                    {divList}
                </div>
            );
        }

        const closeFunc = () => {
            this.logs = null;
            this.setState({
                open: false,
                isLoginIn: false
            });
        };

        if (!(this.state.open && this.state.isLoginIn)) return null;

        return(
            <div className={"ts-root-login-field-div"}>
                <div className={"ts-root-log-field"}>
                    <div className={"CSSBase-TextBig"} style={{textAlign: "center", marginTop: "max(25px, 2.5vh)"}}>
                        Logs

                        <div style={{display: "flex", width: "100%", flexDirection: "row-reverse"}}>
                            <img className={"ts-root-log-button-x"}
                                 src={"close.svg"}
                                 onClick={() => {closeFunc()}}
                            />
                        </div>
                    </div>

                    {Logs()}
                </div>
            </div>
        );
    }

    render() {
        return (
            <div style={{height: height}} className={"ts-root"}>
                {this.buttonLoginDiv()}
                {this.loginFieldDiv()}
                {this.LogFieldDiv()}
            </div>
        );
    }
}
