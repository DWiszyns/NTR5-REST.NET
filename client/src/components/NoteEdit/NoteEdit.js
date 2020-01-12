import React from 'react';
import { Formik } from 'formik';
import Form from 'react-bootstrap/Form';
import moment, {Moment} from 'moment'
import Row from 'react-bootstrap/Row';
import Col from 'react-bootstrap/Col';
import Button from 'react-bootstrap/Button';
import ListGroup from 'react-bootstrap/ListGroup';
import { Link, withRouter } from 'react-router-dom';
import axios from "axios";
const API = 'http://localhost:5000/api';


const NoteEdit = props => {
    const title = props.title || 'Enter title';
    const text = props.content || (props.title? '':'Enter text');
    const markdown = props.markdown || false;
    const date = props.date || moment(new Date()).format("YYYY-MM-DD");
    const mode = props.mode==='new'? props.mode : 'edit';
    const newCategory = '';
    const [selectedCategory, setSelectedCategory] = React.useState('');
    const [removeEnabled, setRemoveEnabled] = React.useState(false);
    const [categories, setCategories] = React.useState(props.noteCategories || []);
    const [errorMessage, setErrorMessage] = React.useState('');


    const validate = values =>{
        values.categories=categories
        const errors = {};
        if (!values.title) {
        errors.title = 'Title is Required';
        }
        if(values.categories.length===0){
            errors.categories='At least one category is required'
        }
        return errors;
    }

    const handleAddCategory = newCategory =>{
        const errors={}
        if(newCategory!=='')
        {
            if(categories.filter(c=>c.title===newCategory).length===0)
                setCategories(categories.concat({title: newCategory}))
        }
        else {
            alert('Can\'t add empty category')
            //return errors
        }
    }

    const selectCategory = c =>{
        setSelectedCategory(c)

    }

    const handleRemoveCategory = e =>{
        if(selectedCategory!=null || selectedCategory !== undefined)
        {
            setCategories(categories.filter(c=>c.title!==selectedCategory))
        }
    }

    const handleOnSubmit = (values) => {
        if(mode==='new'){
            axios
                .post(`${API}/notes`, {
                    title: values.title,
                    text: values.text,
                    markdown: values.markdown,
                    date: values.date,
                    noteCategories: categories,
                })
                .then(res => {
                    if (res.data !== 'Success') {
                        setErrorMessage(res.data);
                    } else {
                        props.history.push('/'); //go back to main menu
                    }
                })
                .catch(err => {
                    console.log(err);
                });

        }
        else{
            axios
                .put(`${API}/notes/${props.title}`,{
                title: values.title,
                text: values.text,
                markdown: values.markdown,
                date: values.date,
                noteCategories: categories,
                }) //old title
                .then(res=> {
                    if (res.data !== 'Success') {
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

    return(
        <div>
            <Formik
                initialValues={{ title, text, date, markdown, categories, newCategory }}
                validate={validate}
                onSubmit={handleOnSubmit}
            >
                {({ handleChange, errors, values, handleSubmit, isSubmitting }) => (
                <Form onSubmit={handleSubmit}>
                    <Form.Group>
                        <Form.Label>Title</Form.Label><br/>
                        <Form.Control
                            type="text"
                            name="title"
                            onChange={handleChange}
                            value={values.title}
                          />
                        <span style={{color:'red'}}>{errors.title}</span>
                        <span style={{color:'red'}}>{errorMessage}</span>
                    </Form.Group>
                    <Form.Group>
                        <Form.Label>Text</Form.Label><br/>
                        <Form.Control
                            type="text"
                            name="text"
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
                        <Form.Label>New category</Form.Label><br/>
                        <Form.Control
                                type="text"
                                name="newCategory"
                                onChange={handleChange}
                                value={values.newCategory}
                            />
                        <Button variant="outline-primary" onClick={() => { handleAddCategory(values.newCategory); values.newCategory = ''; }}
                                title="Add category">Add category</Button>
                    </Form.Group>
                    <Form.Group>
                        <Form.Label>Note's categories</Form.Label><br/>
                        <ListGroup>
                            {categories.map(({title}) => (
                                <Row>
                                    <ListGroup.Item type="button" onClick={() => selectCategory(title)} variant="outline-secondary"
                                                    action key={title}>{title}</ListGroup.Item>
                                </Row>
                            ))}
                        </ListGroup>
                        <Button variant="outline-primary" onClick={handleRemoveCategory} title="Remove category">Remove category</Button>
                        <span style={{color:'red'}}>{errors.categories}</span>
                    </Form.Group>
                    <Button variant="primary" type="submit" title="Submit">Create note</Button>
                </Form>
                )}
            </Formik><br/>
            <Link to={'/'}>
                <Button type="button" variant="dark" >Back to list</Button>
            </Link>
        </div>
    );
};
export default withRouter(NoteEdit);
