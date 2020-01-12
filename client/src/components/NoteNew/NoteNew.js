import React from 'react';
import NoteEdit from '../NoteEdit/NoteEdit';
import { Formik, Form } from 'formik';
import moment, {Moment} from 'moment'
import Row from 'react-bootstrap/Row';
import Col from 'react-bootstrap/Col';
import Button from 'react-bootstrap/Button';
import ListGroup from 'react-bootstrap/ListGroup';
import { Link, withRouter } from 'react-router-dom';


export default function NoteNew() {
    return (
        <div>
        <h1>New Note</h1>
            <NoteEdit mode="new"/>
        </div>
    )
};
