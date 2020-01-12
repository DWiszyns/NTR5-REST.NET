import React, { Component } from 'react';

import logo from './logo.svg';

import './App.css';
import 'bootstrap/dist/css/bootstrap.min.css';

import Container from 'react-bootstrap/Container';
import NoteNew from './components/NoteNew/NoteNew';
import NoteList from './components/NoteList/NoteList';


import { BrowserRouter as Router, Route } from 'react-router-dom';
import NoteContainer from "./containers/NoteContainer";

class App extends Component {

  
  componentDidMount() {

  }

  
render() {
    return (
      <Router>
        <Container>
          <Route exact path="/notes/new" component={NoteNew} />
          <Route exact path="/notes/edit/:title" component={NoteContainer} />
          <Route exact path="/" component={NoteList} />
        </Container>
      </Router>
    );
  }
}

export default App;
