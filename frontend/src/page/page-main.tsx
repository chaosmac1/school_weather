import React from "react";
import {TSMain} from "../template/template-main";
import {IHistory} from "../i-history";
import {IMatch} from "../i-match";

export interface IPropsPageMain {
    history: IHistory,
    match: IMatch
}
export interface IStatePageMain { }
export class PageMain extends React.Component<IPropsPageMain, IStatePageMain> {
    constructor(props:IPropsPageMain) {
        super(props);

        this.state = {  }
    }

    render() {
        return ( <TSMain history={this.props.history} />);
    }
}