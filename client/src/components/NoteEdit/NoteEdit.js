import './NoteEdit.css'
import React from 'react';
import {Formik} from 'formik';
import Form from 'react-bootstrap/Form';
import moment, {Moment} from 'moment'
import Row from 'react-bootstrap/Row';
import Col from 'react-bootstrap/Col';
import Button from 'react-bootstrap/Button';
import ListGroup from 'react-bootstrap/ListGroup';
import {Link, withRouter} from 'react-router-dom';
import axios from "axios";

const API = 'https://localhost:5001';


const NoteEdit = props => {
    const title = props.title || 'Enter title';
    const text = props.content || (props.title ? '' : 'Enter text');
    const markdown = props.markdown || false;
    const date = moment(props.date).format("YYYY-MM-DD") || moment(new Date()).format("YYYY-MM-DD");
    const mode = props.mode === 'new' ? props.mode : 'edit';
    const newCategory = '';
    const [selectedCategory, setSelectedCategory] = React.useState('');
    const [removeEnabled, setRemoveEnabled] = React.useState(false);
    const [categories, setCategories] = React.useState(props.noteCategories || []);
    const [errorMessage, setErrorMessage] = React.useState('');


    const validate = values => {
        values.categories = categories
        const errors = {};
        if (!values.title) {
            errors.title = 'Title is Required';
        }
        if (values.categories.length === 0) {
            errors.categories = 'At least one category is required'
        }
        return errors;
    }

    const handleAddCategory = newCategory => {
        const errors = {}
        if (newCategory !== '') {
            if (categories.filter(c => c === newCategory).length === 0)
                setCategories(categories.concat(newCategory))
        } else {
            alert('Can\'t add empty category')
            //return errors
        }
    }

    const selectCategory = c => {
        setSelectedCategory(c)

    }

    const handleRemoveCategory = e => {
        if (selectedCategory != null || selectedCategory !== undefined) {
            setCategories(categories.filter(c => c !== selectedCategory))
        }
    }

    const handleOnSubmit = (values) => {
        debugger;
        if (mode === 'new') {
            axios
                .post(`${API}/notes`, {
                    title: values.title,
                    text: values.text,
                    markdown: values.markdown,
                    date: values.date,
                    noteCategories: values.categories,
                })
                .then(res => {
                    console.log(res);
                    if (res.status !== 200) {
                        setErrorMessage(res.data);
                    } else {
                        props.history.push('/'); //go back to main menu
                    }
                })
                .catch(err => {
                    console.log(err);
                });

        } else {
            axios
                .put(`${API}/notes/${props.idnote}`, {
                    title: values.title,
                    text: values.text,
                    markdown: values.markdown,
                    date: values.date,
                    noteCategories: values.categories,
                }) //old title
                .then(res => {
                    if (res.status !== 200) {
                        setErrorMessage(res.data);
                    } else {
                        props.history.push('/'); //go back to main menu
                    }
                })
                .catch(err => {
                    console.log(err);
                });

        }
    };

    return (
        <div>
            <Formik
                initialValues={{title, text, date, markdown, categories, newCategory}}
                validate={validate}
                onSubmit={handleOnSubmit}
            >
                {({handleChange, errors, values, handleSubmit, isSubmitting}) => (
                    <Form onSubmit={handleSubmit}>
                        <Form.Group>
                            <Form.Label>Title</Form.Label>
                            <Form.Control
                                type="text"
                                name="title"
                                onChange={handleChange}
                                value={values.title}
                                style={{width: '300px'}}
                            />
                            <span style={{color: 'red'}}>{errors.title}</span>
                            <span style={{color: 'red'}}>{errorMessage}</span>
                        </Form.Group>
                        <Form.Check controlId="formNoteMarkdown">
                            <Form.Check.Input
                                type="checkbox"
                                name="markdown"
                                checked={values.markdown}
                                onChange={handleChange}
                                value={values.markdown}
                            />
                            <Form.Label>Markdown</Form.Label>
                        </Form.Check>
                        <Form.Group>
                            <Form.Label>Text</Form.Label><br/>
                            <Form.Control
                                type="text"
                                name="text"
                                as="textarea"
                                rows={7}
                                cols={50}
                                style={{resize: 'none'}}
                                onChange={handleChange}
                                value={values.text}
                            />
                        </Form.Group>
                        <Form.Group>
                            <Form.Label>Date</Form.Label><br/>
                            <Form.Control
                                type="date"
                                name="date"
                                onChange={handleChange}
                                value={values.date}
                            />
                        </Form.Group>
                        <Form.Row>
                            <Form.Group style={{marginRight: "300px"}}>
                                <Form.Label>New category</Form.Label><br/>
                                <Form.Control
                                    type="text"
                                    name="newCategory"
                                    onChange={handleChange}
                                    value={values.newCategory}
                                />
                                <Button variant="outline-primary" onClick={() => {
                                    handleAddCategory(values.newCategory);
                                    values.newCategory = '';
                                }}
                                        title="Add category">Add category</Button>
                            </Form.Group>
                            <Form.Group style={{marginRight: "50px"}}>
                                <Form.Label>Note's categories</Form.Label><br/>
                                <div className="categoriesList">
                                    <ListGroup>
                                        {categories.map((title) => (
                                            <Row>
                                                <ListGroup.Item type="button" onClick={() => selectCategory(title)}
                                                                variant="outline-secondary"
                                                                style={{width: '500px'}}
                                                                action key={title}>{title}</ListGroup.Item>
                                            </Row>
                                        ))}
                                    </ListGroup>
                                </div>
                                <Button variant="outline-primary" style={{height: "50px"}} onClick={handleRemoveCategory} title="Remove category">Remove
                                    category</Button>
                            </Form.Group>
                            <span style={{color: 'red'}}>{errors.categories}</span>
                        </Form.Row>

                        <Row>
                            <Button variant="primary" type="submit" style={{marginRight: "5px"}} title="Submit">Create
                                note</Button>
                            <Link to={'/'}>
                                <Button type="button" variant="dark">Back to list</Button>
                            </Link>
                        </Row>
                    </Form>
                )}
            </Formik><br/>

        </div>
    );
};
export default withRouter(NoteEdit);
