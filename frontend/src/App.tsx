import React from 'react';
import {BrowserRouter as Router, Route} from 'react-router-dom';
import {PageMain} from "./page/page-main";
import "./CSSBase.css"

function App() {
  return (
    <Router>
      <Route path={"/"} component={PageMain}/>
    </Router>
  );
}

export default App;
