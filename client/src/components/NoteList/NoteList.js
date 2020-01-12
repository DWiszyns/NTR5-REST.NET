import React,{Component} from 'react';
import Form from 'react-bootstrap/Form';
import moment from 'moment'
import Button from 'react-bootstrap/Button';
import { Link, withRouter } from 'react-router-dom';
import Table from 'react-bootstrap/Table'
import axios from 'axios';
import Row from 'react-bootstrap/Row';
import Col from 'react-bootstrap/Col';
const API = 'http://localhost:5000/api';



class NoteList extends  Component {
    constructor(props) {
        super(props);
        this.state={
            dateFrom: localStorage.getItem("dateFrom") || moment(new Date().setMonth(2)).format("YYYY-MM-DD"),
            dateTo: localStorage.getItem("dateTo")  || moment(new Date()).format("YYYY-MM-DD"),
            category: localStorage.getItem("category")  || '',
            page: localStorage.getItem("page")  || 1,
            notes:[],
            categories:[],
            pager:{}
        }
    }

    loadPage(){
        //const params = new URLSearchParams(window.location.search);
        let respon={}
        axios
            .get(
                `${API}/notes?page=${this.state.page}&category=${this.state.category}&dateFrom=${this.state.dateFrom &&
                moment(this.state.dateFrom).format("YYYY-MM-DD")}&dateTo=${this.state.dateTo &&
                moment(this.state.dateTo).format("YYYY-MM-DD")}`
            )
            .then(res => respon=res.data)
            .then(({ pager, notes, categories }) => {
                const currState = this.state;
                currState.pager=pager;
                currState.notes=notes ? JSON.parse(JSON.stringify(notes)) : [];
                currState.categories=categories ? [{title:''}].concat(JSON.parse(JSON.stringify(categories))):[];
                localStorage.setItem("page",this.state.page)
                localStorage.setItem("dateTo",this.state.dateTo)
                localStorage.setItem("dateFrom",this.state.dateFrom)
                localStorage.setItem("category",this.state.category)

                this.setState({currState})
            })
            .catch(err => {
            console.log(err);
        });
        console.log(this.state.dateFrom)
        console.log(this.state.pager)
        console.log(this.state.notes)
        console.log(this.state.categories)
        console.log(respon)
    };
    componentDidMount(){
        console.log("I'm called")
        this.loadPage();
    }

    deleteNote = title => {
        axios
            .delete(`${API}/notes/${title}`)
            .then(res => {
                if (res.data === 'Success') {
                    this.loadPage()
                }
            })
            .catch(err => {
                console.log(err);
            });
    };

    handleSubmit=(e) =>{
        e.preventDefault();
        const formValues  = this.state;
        for(let i=0;i<e.target.childElementCount;++i)
            formValues[e.target[i].name] = e.target[i].value
        this.setState({ formValues });
        this.loadPage()
    };

    handleChange = ({ target }) => {
        const formValues  = this.state;
        formValues[target.name] = target.value;
        this.setState({ formValues });
    };
    previousPage=()=>{
        const formValues  = this.state;
        formValues.page=formValues.page-1;
        this.setState({ formValues });
        this.loadPage()
    };

    nextPage=()=>{
        const formValues  = this.state;
        formValues.page=formValues.page+1;
        this.setState({ formValues });
        this.loadPage()
    };

    render(){
        return <div>
            {console.log('I render')}
            <h1>Note's list</h1>
                    <Form onSubmit={this.handleSubmit}>
                        <Form.Group>
                            <Form.Label>Date from</Form.Label><br/>
                            <Form.Control
                                type="date"
                                name="dateFrom"
                                onChange={e => {
                                    this.handleChange(e);
                                }}
                                value={this.state.dateFrom}
                            />
                        </Form.Group>
                        <Form.Group>
                            <Form.Label>Date to</Form.Label><br/>
                            <Form.Control
                                type="date"
                                name="dateTo"
                                onChange={e => {
                                    this.handleChange(e);
                                }}
                                value={this.state.dateTo}
                            />
                        </Form.Group>
                        <Form.Group>
                            <Form.Label>Category</Form.Label><br/>
                            <Form.Control as="select" name="category"
                                          onChange={e => {
                                              this.handleChange(e);
                                          }}
                                          value={this.state.category}>
                                {this.state.categories.map(c => (
                                        <option>{c.title}</option>
                                    )
                                )}
                            </Form.Control>
                        </Form.Group>
                        <Button variant="primary" type="submit" title="Submit">Filter</Button>
                    </Form>
            <Table striped bordered hover>
                <thead>
                <tr>
                    <th>Title</th>
                    <th>Date</th>
                </tr>
                </thead>
                <tbody>
                {this.state.notes.map(n => (
                        <tr>
                            <td>{n.title}</td>
                            <td>{n.date}</td>
                            <td>
                                <Link to={`/notes/edit/${n.title}`}>
                                    <Button type="button" variant="secondary">Edit</Button>
                                </Link>
                                <Button type="button" variant="secondary" onClick={() => this.deleteNote(n.title)}>Delete</Button>
                            </td>
                        </tr>
                    )
                )}
                </tbody>
            </Table>
                <Row>
                    <Col>
                        <Link to={'/notes/new'}>
                            <Button type="button" variant="primary">New note</Button>
                        </Link>
                    </Col>
                    <Button
                        variant="secondary"
                        onClick={() => this.previousPage()}
                        disabled={this.state.pager.currentPage === 1 || !this.state.pager.currentPage}
                    >
                        Prev Page
                    </Button>
                    {`${this.state.pager.currentPage || 1} / ${this.state.pager.endPage || 1}`}
                    <Button
                        variant="secondary"
                        onClick={() => this.nextPage()}
                        disabled={this.state.pager.currentPage === this.state.pager.endPage || !this.state.pager.currentPage}
                    >
                        Next Page
                    </Button>
                </Row>
        </div>
    }
}

export default withRouter(NoteList);
