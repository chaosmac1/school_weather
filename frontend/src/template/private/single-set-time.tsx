import React, {ChangeEvent, CSSProperties} from "react";
import {IStateTSMain} from "../template-main";
import "../../CSSBase.css"
import imgSelectData from "./select.gif";
import {SICheckbox} from "../../single/single-Checkbox";

interface IPropsSISetTime {
    setMainState: (props: { dateStart: Date; dateEnd: Date; manuel: boolean; diff: string }) => void;
}

interface IStateSISetTime { }

