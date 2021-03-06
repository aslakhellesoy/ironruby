/* ****************************************************************************
 *
 * Copyright (c) Microsoft Corporation. 
 *
 * This source code is subject to terms and conditions of the Microsoft Public License. A 
 * copy of the license can be found in the License.html file at the root of this distribution. If 
 * you cannot locate the  Microsoft Public License, please send an email to 
 * dlr@microsoft.com. By using this source code in any fashion, you are agreeing to be bound 
 * by the terms of the Microsoft Public License.
 *
 * You must not remove this notice, or any other, from this software.
 *
 *
 * ***************************************************************************/

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic.Utils;
using System.Text;

namespace System.Linq.Expressions {
    //CONFORMING
    /// <summary>
    /// Represents creating a new array and possibly initializing the elements of the new array.
    /// </summary>
    public class NewArrayExpression : Expression {
        private readonly ReadOnlyCollection<Expression> _expressions;
        private readonly Type _type;

        internal NewArrayExpression(Type type, ReadOnlyCollection<Expression> expressions) {
            _expressions = expressions;
            _type = type;
        }

        internal static NewArrayExpression Make(ExpressionType nodeType, Type type, ReadOnlyCollection<Expression> expressions) {
            if (nodeType == ExpressionType.NewArrayInit) {
                return new NewArrayInitExpression(type, expressions);
            } else {
                return new NewArrayBoundsExpression(type, expressions);
            }
        }

        /// <summary>
        /// Gets the static type of the expression that this <see cref="Expression" /> represents. (Inherited from <see cref="Expression"/>.)
        /// </summary>
        /// <returns>The <see cref="Type"/> that represents the static type of the expression.</returns>
        protected override Type GetExpressionType() {
            return _type;
        }

        /// <summary>
        /// Gets the bounds of the array if the value of the <see cref="P:NodeType"/> property is NewArrayBounds, or the values to initialize the elements of the new array if the value of the <see cref="P:NodeType"/> property is NewArrayInit. 
        /// </summary>
        public ReadOnlyCollection<Expression> Expressions {
            get { return _expressions; }
        }

        internal override Expression Accept(ExpressionVisitor visitor) {
            return visitor.VisitNewArray(this);
        }
    }

    internal sealed class NewArrayInitExpression : NewArrayExpression {
        internal NewArrayInitExpression(Type type, ReadOnlyCollection<Expression> expressions)
            : base(type, expressions) {
        }


        /// <summary>
        /// Returns the node type of this <see cref="Expression" />. (Inherited from <see cref="Expression" />.)
        /// </summary>
        /// <returns>The <see cref="ExpressionType"/> that represents this expression.</returns>
        protected override ExpressionType GetNodeKind() {
            return ExpressionType.NewArrayInit;
        }
    }

    internal sealed class NewArrayBoundsExpression : NewArrayExpression {
        internal NewArrayBoundsExpression(Type type, ReadOnlyCollection<Expression> expressions)
            : base(type, expressions) {
        }

        /// <summary>
        /// Returns the node type of this <see cref="Expression" />. (Inherited from <see cref="Expression" />.)
        /// </summary>
        /// <returns>The <see cref="ExpressionType"/> that represents this expression.</returns>
        protected override ExpressionType GetNodeKind() {
            return ExpressionType.NewArrayBounds;
        }
    }
    
    public partial class Expression {

        #region NewArrayInit

        //CONFORMING
        /// <summary>
        /// Creates a new array expression of the specified type from the provided initializers.
        /// </summary>
        /// <param name="type">A Type that represents the element type of the array.</param>
        /// <param name="initializers">The expressions used to create the array elements.</param>
        /// <returns>An instance of the <see cref="NewArrayExpression"/>.</returns>
        public static NewArrayExpression NewArrayInit(Type type, params Expression[] initializers) {
            return NewArrayInit(type, (IEnumerable<Expression>)initializers);
        }

        /// <summary>
        /// Creates a new array expression of the specified type from the provided initializers.
        /// </summary>
        /// <param name="type">A Type that represents the element type of the array.</param>
        /// <param name="initializers">The expressions used to create the array elements.</param>
        /// <returns>An instance of the <see cref="NewArrayExpression"/>.</returns>
        public static NewArrayExpression NewArrayInit(Type type, IEnumerable<Expression> initializers) {
            ContractUtils.RequiresNotNull(type, "type");
            ContractUtils.RequiresNotNull(initializers, "initializers");
            if (type.Equals(typeof(void))) {
                throw Error.ArgumentCannotBeOfTypeVoid();
            }

            ReadOnlyCollection<Expression> initializerList = initializers.ToReadOnly();

            Expression[] newList = null;
            for (int i = 0, n = initializerList.Count; i < n; i++) {
                Expression expr = initializerList[i];
                RequiresCanRead(expr, "initializers");

                if (!TypeUtils.AreReferenceAssignable(type, expr.Type)) {
                    if (TypeUtils.IsSameOrSubclass(typeof(Expression), type) && type.IsAssignableFrom(expr.GetType())) {
                        expr = Expression.Quote(expr);
                    } else {
                        throw Error.ExpressionTypeCannotInitializeArrayType(expr.Type, type);
                    }
                    if (newList == null) {
                        newList = new Expression[initializerList.Count];
                        for (int j = 0; j < i; j++) {
                            newList[j] = initializerList[j];
                        }
                    }
                }
                if (newList != null) {
                    newList[i] = expr;
                }
            }
            if (newList != null) {
                initializerList = new ReadOnlyCollection<Expression>(newList);
            }

            return NewArrayExpression.Make(ExpressionType.NewArrayInit, type.MakeArrayType(), initializerList);
        }

        #endregion

        #region NewArrayBounds

        //CONFORMING
        /// <summary>
        /// Creates a <see cref="NewArrayExpression"/> that represents creating an array that has a specified rank. 
        /// </summary>
        /// <param name="type">A <see cref="Type"/> that represents the element type of the array.</param>
        /// <param name="bounds">An array that contains Expression objects to use to populate the Expressions collection.</param>
        /// <returns>A <see cref="NewArrayExpression"/> that has the <see cref="P:NodeType"/> property equal to type and the <see cref="P:Expressions"/> property set to the specified value.</returns>
        public static NewArrayExpression NewArrayBounds(Type type, params Expression[] bounds) {
            return NewArrayBounds(type, (IEnumerable<Expression>)bounds);
        }

        //CONFORMING
        /// <summary>
        /// Creates a <see cref="NewArrayExpression"/> that represents creating an array that has a specified rank. 
        /// </summary>
        /// <param name="type">A <see cref="Type"/> that represents the element type of the array.</param>
        /// <param name="bounds">An IEnumerable{T} that contains Expression objects to use to populate the Expressions collection.</param>
        /// <returns>A <see cref="NewArrayExpression"/> that has the <see cref="P:NodeType"/> property equal to type and the <see cref="P:Expressions"/> property set to the specified value.</returns>
        public static NewArrayExpression NewArrayBounds(Type type, IEnumerable<Expression> bounds) {
            ContractUtils.RequiresNotNull(type, "type");
            ContractUtils.RequiresNotNull(bounds, "bounds");

            if (type.Equals(typeof(void))) {
                throw Error.ArgumentCannotBeOfTypeVoid();
            }

            ReadOnlyCollection<Expression> boundsList = bounds.ToReadOnly();

            int dimensions = boundsList.Count;
            ContractUtils.Requires(dimensions > 0, "bounds", Strings.BoundsCannotBeLessThanOne);

            for (int i = 0; i < dimensions; i++) {
                Expression expr = boundsList[i];
                RequiresCanRead(expr, "bounds");
                if (!TypeUtils.IsInteger(expr.Type)) {
                    throw Error.ArgumentMustBeInteger();
                }
            }

            Type arrayType;
            if (dimensions == 1) {
                //To get a vector, need call Type.MakeArrayType(). 
                //Type.MakeArrayType(1) gives a non-vector array, which will cause type check error.
                arrayType = type.MakeArrayType();
            } else {
                arrayType = type.MakeArrayType(dimensions);
            }

            return NewArrayExpression.Make(ExpressionType.NewArrayBounds, arrayType, bounds.ToReadOnly());
        }

        #endregion

    }
}
