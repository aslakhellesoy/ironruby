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

using System.Dynamic.Utils;
using System.Text;

namespace System.Linq.Expressions {
    /// <summary>
    /// Specifies what kind of jump this <see cref="GotoExpression"/> represents.
    /// </summary>
    public enum GotoExpressionKind {
        /// <summary>
        /// A <see cref="GotoExpression"/> that represents a jump to some location.
        /// </summary>
        Goto,
        /// <summary>
        /// A <see cref="GotoExpression"/> that represents a return statement.
        /// </summary>
        Return,
        /// <summary>
        /// A <see cref="GotoExpression"/> that represents a break statement.
        /// </summary>
        Break,
        /// <summary>
        /// A <see cref="GotoExpression"/> that represents a continue statement.
        /// </summary>
        Continue,
    }

    /// <summary>
    /// Represents an unconditional jump. This includes return statements, break and continue statements, and other jumps.
    /// </summary>
    public sealed class GotoExpression : Expression {
        private readonly GotoExpressionKind _kind;
        private readonly Expression _value;
        private readonly LabelTarget _target;
        private readonly Type _type;

        internal GotoExpression(GotoExpressionKind kind, LabelTarget target, Expression value, Type type) {
            _kind = kind;
            _value = value;
            _target = target;
            _type = type;
        }

        /// <summary>
        /// Gets the static type of the expression that this <see cref="Expression" /> represents. (Inherited from <see cref="Expression"/>.)
        /// </summary>
        /// <returns>The <see cref="Type"/> that represents the static type of the expression.</returns>
        protected override Type GetExpressionType() {
            return _type;
        }

        /// <summary>
        /// Returns the node type of this <see cref="Expression" />. (Inherited from <see cref="Expression" />.)
        /// </summary>
        /// <returns>The <see cref="ExpressionType"/> that represents this expression.</returns>
        protected override ExpressionType GetNodeKind() {
            return ExpressionType.Goto;
        }

        /// <summary>
        /// The value passed to the target, or null if the target is of type
        /// System.Void.
        /// </summary>
        public Expression Value {
            get { return _value; }
        }

        /// <summary>
        /// The target label where this node jumps to.
        /// </summary>
        public LabelTarget Target {
            get { return _target; }
        }

        /// <summary>
        /// The kind of the goto. For information purposes only.
        /// </summary>
        public GotoExpressionKind Kind {
            get { return _kind; }
        }

        internal override Expression Accept(ExpressionVisitor visitor) {
            return visitor.VisitGoto(this);
        }
    }

    public partial class Expression {
        /// <summary>
        /// Creates a <see cref="GotoExpression"/> representing a break statement.
        /// </summary>
        /// <param name="target">The <see cref="LabelTarget"/> that the <see cref="GotoExpression"/> will jump to.</param>
        /// <returns>
        /// A <see cref="GotoExpression"/> with <see cref="P:GotoExpression.Kind"/> equal to Break, 
        /// the <see cref="P:GotoExpression.Target"/> property set to <paramref name="target"/>, and a null value to be passed to the target label upon jumping.
        /// </returns>
        public static GotoExpression Break(LabelTarget target) {
            return MakeGoto(GotoExpressionKind.Break, target, null, typeof(void));
        }

        /// <summary>
        /// Creates a <see cref="GotoExpression"/> representing a break statement. The value passed to the label upon jumping can be specified.
        /// </summary>
        /// <param name="target">The <see cref="LabelTarget"/> that the <see cref="GotoExpression"/> will jump to.</param>
        /// <param name="value">The value that will be passed to the associated label upon jumping.</param>
        /// <returns>
        /// A <see cref="GotoExpression"/> with <see cref="P:GotoExpression.Kind"/> equal to Break, 
        /// the <see cref="P:GotoExpression.Target"/> property set to <paramref name="target"/>, 
        /// and <paramref name="value"/> to be passed to the target label upon jumping.
        /// </returns>
        public static GotoExpression Break(LabelTarget target, Expression value) {
            return MakeGoto(GotoExpressionKind.Break, target, value, typeof(void));
        }

        /// <summary>
        /// Creates a <see cref="GotoExpression"/> representing a break statement with the specified type.
        /// </summary>
        /// <param name="target">The <see cref="LabelTarget"/> that the <see cref="GotoExpression"/> will jump to.</param>
        /// <param name="type">An <see cref="System.Type"/> to set the <see cref="P:Expression.Type"/> property equal to.</param>
        /// <returns>
        /// A <see cref="GotoExpression"/> with <see cref="P:GotoExpression.Kind"/> equal to Break, 
        /// the <see cref="P:GotoExpression.Target"/> property set to <paramref name="target"/>, 
        /// and the <see cref="P:Expression.Type"/> property set to <paramref name="type"/>.
        /// </returns>
        public static GotoExpression Break(LabelTarget target, Type type) {
            return MakeGoto(GotoExpressionKind.Break, target, null, type);
        }

        /// <summary>
        /// Creates a <see cref="GotoExpression"/> representing a break statement with the specified type. 
        /// The value passed to the label upon jumping can be specified.
        /// </summary>
        /// <param name="target">The <see cref="LabelTarget"/> that the <see cref="GotoExpression"/> will jump to.</param>
        /// <param name="value">The value that will be passed to the associated label upon jumping.</param>
        /// <param name="type">An <see cref="System.Type"/> to set the <see cref="P:Expression.Type"/> property equal to.</param>
        /// <returns>
        /// A <see cref="GotoExpression"/> with <see cref="P:GotoExpression.Kind"/> equal to Break, 
        /// the <see cref="P:GotoExpression.Target"/> property set to <paramref name="target"/>, 
        /// the <see cref="P:Expression.Type"/> property set to <paramref name="type"/>,
        /// and <paramref name="value"/> to be passed to the target label upon jumping.
        /// </returns>
        public static GotoExpression Break(LabelTarget target, Expression value, Type type) {
            return MakeGoto(GotoExpressionKind.Break, target, value, type);
        }

        /// <summary>
        /// Creates a <see cref="GotoExpression"/> representing a continue statement.
        /// </summary>
        /// <param name="target">The <see cref="LabelTarget"/> that the <see cref="GotoExpression"/> will jump to.</param>
        /// <returns>
        /// A <see cref="GotoExpression"/> with <see cref="P:GotoExpression.Kind"/> equal to Continue, 
        /// the <see cref="P:GotoExpression.Target"/> property set to <paramref name="target"/>, 
        /// and a null value to be passed to the target label upon jumping.
        /// </returns>
        public static GotoExpression Continue(LabelTarget target) {
            return MakeGoto(GotoExpressionKind.Continue, target, null, typeof(void));
        }

        /// <summary>
        /// Creates a <see cref="GotoExpression"/> representing a continue statement with the specified type.
        /// </summary>
        /// <param name="target">The <see cref="LabelTarget"/> that the <see cref="GotoExpression"/> will jump to.</param>
        /// <param name="type">An <see cref="System.Type"/> to set the <see cref="P:Expression.Type"/> property equal to.</param>
        /// <returns>
        /// A <see cref="GotoExpression"/> with <see cref="P:GotoExpression.Kind"/> equal to Continue, 
        /// the <see cref="P:GotoExpression.Target"/> property set to <paramref name="target"/>, 
        /// the <see cref="P:Expression.Type"/> property set to <paramref name="type"/>,
        /// and a null value to be passed to the target label upon jumping.
        /// </returns>
        public static GotoExpression Continue(LabelTarget target, Type type) {
            return MakeGoto(GotoExpressionKind.Continue, target, null, type);
        }

        /// <summary>
        /// Creates a <see cref="GotoExpression"/> representing a return statement.
        /// </summary>
        /// <param name="target">The <see cref="LabelTarget"/> that the <see cref="GotoExpression"/> will jump to.</param>
        /// <returns>
        /// A <see cref="GotoExpression"/> with <see cref="P:GotoExpression.Kind"/> equal to Return, 
        /// the <see cref="P:GotoExpression.Target"/> property set to <paramref name="target"/>, 
        /// and a null value to be passed to the target label upon jumping.
        /// </returns>
        public static GotoExpression Return(LabelTarget target) {
            return MakeGoto(GotoExpressionKind.Return, target, null, typeof(void));
        }

        /// <summary>
        /// Creates a <see cref="GotoExpression"/> representing a return statement with the specified type.
        /// </summary>
        /// <param name="target">The <see cref="LabelTarget"/> that the <see cref="GotoExpression"/> will jump to.</param>
        /// <param name="type">An <see cref="System.Type"/> to set the <see cref="P:Expression.Type"/> property equal to.</param>
        /// <returns>
        /// A <see cref="GotoExpression"/> with <see cref="P:GotoExpression.Kind"/> equal to Return, 
        /// the <see cref="P:GotoExpression.Target"/> property set to <paramref name="target"/>, 
        /// the <see cref="P:Expression.Type"/> property set to <paramref name="type"/>,
        /// and a null value to be passed to the target label upon jumping.
        /// </returns>
        public static GotoExpression Return(LabelTarget target, Type type) {
            return MakeGoto(GotoExpressionKind.Return, target, null, type);
        }

        /// <summary>
        /// Creates a <see cref="GotoExpression"/> representing a return statement. The value passed to the label upon jumping can be specified.
        /// </summary>
        /// <param name="target">The <see cref="LabelTarget"/> that the <see cref="GotoExpression"/> will jump to.</param>
        /// <param name="value">The value that will be passed to the associated label upon jumping.</param>
        /// <returns>
        /// A <see cref="GotoExpression"/> with <see cref="P:GotoExpression.Kind"/> equal to Continue, 
        /// the <see cref="P:GotoExpression.Target"/> property set to <paramref name="target"/>, 
        /// and <paramref name="value"/> to be passed to the target label upon jumping.
        /// </returns>
        public static GotoExpression Return(LabelTarget target, Expression value) {
            return MakeGoto(GotoExpressionKind.Return, target, value, typeof(void));
        }

        /// <summary>
        /// Creates a <see cref="GotoExpression"/> representing a return statement with the specified type. 
        /// The value passed to the label upon jumping can be specified.
        /// </summary>
        /// <param name="target">The <see cref="LabelTarget"/> that the <see cref="GotoExpression"/> will jump to.</param>
        /// <param name="value">The value that will be passed to the associated label upon jumping.</param>
        /// <param name="type">An <see cref="System.Type"/> to set the <see cref="P:Expression.Type"/> property equal to.</param>
        /// <returns>
        /// A <see cref="GotoExpression"/> with <see cref="P:GotoExpression.Kind"/> equal to Continue, 
        /// the <see cref="P:GotoExpression.Target"/> property set to <paramref name="target"/>, 
        /// the <see cref="P:Expression.Type"/> property set to <paramref name="type"/>,
        /// and <paramref name="value"/> to be passed to the target label upon jumping.
        /// </returns>
        public static GotoExpression Return(LabelTarget target, Expression value, Type type) {
            return MakeGoto(GotoExpressionKind.Return, target, value, type);
        }
        
        /// <summary>
        /// Creates a <see cref="GotoExpression"/> representing a goto.
        /// </summary>
        /// <param name="target">The <see cref="LabelTarget"/> that the <see cref="GotoExpression"/> will jump to.</param>
        /// <returns>
        /// A <see cref="GotoExpression"/> with <see cref="P:GotoExpression.Kind"/> equal to Goto, 
        /// the <see cref="P:GotoExpression.Target"/> property set to the specified value, 
        /// and a null value to be passed to the target label upon jumping.
        /// </returns>
        public static GotoExpression Goto(LabelTarget target) {
            return MakeGoto(GotoExpressionKind.Goto, target, null, typeof(void));
        }

        /// <summary>
        /// Creates a <see cref="GotoExpression"/> representing a goto with the specified type.
        /// </summary>
        /// <param name="target">The <see cref="LabelTarget"/> that the <see cref="GotoExpression"/> will jump to.</param>
        /// <param name="type">An <see cref="System.Type"/> to set the <see cref="P:Expression.Type"/> property equal to.</param>
        /// <returns>
        /// A <see cref="GotoExpression"/> with <see cref="P:GotoExpression.Kind"/> equal to Goto, 
        /// the <see cref="P:GotoExpression.Target"/> property set to the specified value, 
        /// the <see cref="P:Expression.Type"/> property set to <paramref name="type"/>,
        /// and a null value to be passed to the target label upon jumping.
        /// </returns>
        public static GotoExpression Goto(LabelTarget target, Type type) {
            return MakeGoto(GotoExpressionKind.Goto, target, null, type);
        }

        /// <summary>
        /// Creates a <see cref="GotoExpression"/> representing a goto. The value passed to the label upon jumping can be specified.
        /// </summary>
        /// <param name="target">The <see cref="LabelTarget"/> that the <see cref="GotoExpression"/> will jump to.</param>
        /// <param name="value">The value that will be passed to the associated label upon jumping.</param>
        /// <returns>
        /// A <see cref="GotoExpression"/> with <see cref="P:GotoExpression.Kind"/> equal to Goto, 
        /// the <see cref="P:GotoExpression.Target"/> property set to <paramref name="target"/>, 
        /// and <paramref name="value"/> to be passed to the target label upon jumping.
        /// </returns>
        public static GotoExpression Goto(LabelTarget target, Expression value) {
            return MakeGoto(GotoExpressionKind.Goto, target, value, typeof(void));
        }

        /// <summary>
        /// Creates a <see cref="GotoExpression"/> representing a goto with the specified type.
        /// The value passed to the label upon jumping can be specified.
        /// </summary>
        /// <param name="target">The <see cref="LabelTarget"/> that the <see cref="GotoExpression"/> will jump to.</param>
        /// <param name="value">The value that will be passed to the associated label upon jumping.</param>
        /// <param name="type">An <see cref="System.Type"/> to set the <see cref="P:Expression.Type"/> property equal to.</param>
        /// <returns>
        /// A <see cref="GotoExpression"/> with <see cref="P:GotoExpression.Kind"/> equal to Goto, 
        /// the <see cref="P:GotoExpression.Target"/> property set to <paramref name="target"/>, 
        /// the <see cref="P:Expression.Type"/> property set to <paramref name="type"/>,
        /// and <paramref name="value"/> to be passed to the target label upon jumping.
        /// </returns>
        public static GotoExpression Goto(LabelTarget target, Expression value, Type type) {
            return MakeGoto(GotoExpressionKind.Goto, target, value, type);
        }

        /// <summary>
        /// Creates a <see cref="GotoExpression"/> representing a jump of the specified <see cref="GotoExpressionKind"/>.
        /// The value passed to the label upon jumping can also be specified.
        /// </summary>
        /// <param name="kind">The <see cref="GotoExpressionKind"/> of the <see cref="GotoExpression"/>.</param>
        /// <param name="target">The <see cref="LabelTarget"/> that the <see cref="GotoExpression"/> will jump to.</param>
        /// <param name="value">The value that will be passed to the associated label upon jumping.</param>
        /// <param name="type">An <see cref="System.Type"/> to set the <see cref="P:Expression.Type"/> property equal to.</param>
        /// <returns>
        /// A <see cref="GotoExpression"/> with <see cref="P:GotoExpression.Kind"/> equal to <paramref name="kind"/>, 
        /// the <see cref="P:GotoExpression.Target"/> property set to <paramref name="target"/>, 
        /// the <see cref="P:Expression.Type"/> property set to <paramref name="type"/>,
        /// and <paramref name="value"/> to be passed to the target label upon jumping.
        /// </returns>
        public static GotoExpression MakeGoto(GotoExpressionKind kind, LabelTarget target, Expression value, Type type) {
            ValidateGoto(target, ref value, "target", "value");
            return new GotoExpression(kind, target, value, type);
        }

        private static void ValidateGoto(LabelTarget target, ref Expression value, string targetParameter, string valueParameter) {
            ContractUtils.RequiresNotNull(target, targetParameter);
            if (value == null) {
                ContractUtils.Requires(target.Type == typeof(void), Strings.LabelMustBeVoidOrHaveExpression);
            } else {
                ValidateGotoType(target.Type, ref value, valueParameter);
            }
        }

        // Standard argument validation, taken from ValidateArgumentTypes
        private static void ValidateGotoType(Type expectedType, ref Expression value, string paramName) {
            RequiresCanRead(value, paramName);
            if (!TypeUtils.AreReferenceAssignable(expectedType, value.Type)) {
                // C# autoquotes return values, so we'll do that here
                if (TypeUtils.IsSameOrSubclass(typeof(Expression), expectedType) &&
                    expectedType.IsAssignableFrom(value.GetType())) {
                    value = Expression.Quote(value);
                }
                throw Error.ExpressionTypeDoesNotMatchLabel(value.Type, expectedType);
            }
        }

    }
}
