using System.Collections;

namespace AWUI.Helper;

/// <summary>
/// 通用矩阵类，支持任意维度的矩阵运算
/// </summary>
public class Matrix : IEnumerable<double>
{
    private readonly double[,] data;

    public int Rows { get; }
    public int Columns { get; }
    public bool IsSquare => Rows == Columns;

    public double this[int row, int col]
    {
        get => data[row, col];
        set => data[row, col] = value;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public IEnumerator<double> GetEnumerator()
    {
        for (int i = 0; i < data.GetLength(0); i++)
        {
            for (int j = 0; j < data.GetLength(1); j++)
            {
                yield return data[i, j];
            }
        }
    }

    #region 构造函数

    public Matrix(int rows, int cols)
    {
        if (rows <= 0 || cols <= 0)
            throw new ArgumentException("行列数必须大于0");

        Rows = rows;
        Columns = cols;
        data = new double[rows, cols];
    }

    #endregion

    #region 赋值

    public Matrix Fill(int row, int col, double value)
    {
        this[row, col] = value;
        return this;
    }

    #endregion

    #region 静态工厂方法

    public static Matrix Identity(int size)
    {
        var matrix = new Matrix(size, size);
        for (int i = 0; i < size; i++)
        {
            matrix[i, i] = 1;
        }
        return matrix;
    }

    public static Matrix Zero(int rows, int cols)
    {
        return new Matrix(rows, cols);
    }

    public static Matrix From2DArray(double[] array, int rows, int cols)
    {
        if (array == null)
            throw new ArgumentNullException(nameof(array));
        if (array.Length != rows * cols)
            throw new ArgumentException("数组长度必须等于rows*cols");

        var matrix = new Matrix(rows, cols);
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                matrix[i, j] = array[i * cols + j];
            }
        }
        return matrix;
    }

    #endregion

    #region 基本运算

    public static Matrix operator +(Matrix a, Matrix b)
    {
        if (a.Rows != b.Rows || a.Columns != b.Columns)
            throw new ArgumentException("矩阵维度必须相同");

        var result = new Matrix(a.Rows, a.Columns);
        for (int i = 0; i < a.Rows; i++)
        {
            for (int j = 0; j < a.Columns; j++)
            {
                result[i, j] = a[i, j] + b[i, j];
            }
        }
        return result;
    }

    public static Matrix operator -(Matrix a, Matrix b)
    {
        if (a.Rows != b.Rows || a.Columns != b.Columns)
            throw new ArgumentException("矩阵维度必须相同");

        var result = new Matrix(a.Rows, a.Columns);
        for (int i = 0; i < a.Rows; i++)
        {
            for (int j = 0; j < a.Columns; j++)
            {
                result[i, j] = a[i, j] - b[i, j];
            }
        }
        return result;
    }

    public static Matrix operator *(Matrix a, Matrix b)
    {
        if (a.Columns != b.Rows)
            throw new ArgumentException("第一个矩阵的列数必须等于第二个矩阵的行数");

        var result = new Matrix(a.Rows, b.Columns);
        for (int i = 0; i < a.Rows; i++)
        {
            for (int j = 0; j < b.Columns; j++)
            {
                double sum = 0;
                for (int k = 0; k < a.Columns; k++)
                {
                    sum += a[i, k] * b[k, j];
                }
                result[i, j] = sum;
            }
        }
        return result;
    }

    public static Matrix operator *(Matrix matrix, double scalar)
    {
        var result = new Matrix(matrix.Rows, matrix.Columns);
        for (int i = 0; i < matrix.Rows; i++)
        {
            for (int j = 0; j < matrix.Columns; j++)
            {
                result[i, j] = matrix[i, j] * scalar;
            }
        }
        return result;
    }

    public static Matrix operator *(double scalar, Matrix matrix)
    {
        return matrix * scalar;
    }

    public override bool Equals(object? obj)
    {
        if (obj is not Matrix other)
            return false;
        if (Rows != other.Rows || Columns != other.Columns)
            return false;
        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Columns; j++)
            {
                if (!this[i, j].Equals(other[i, j]))
                    return false;
            }
        }
        return true;
    }

    public static bool operator ==(Matrix left, Matrix right)
    {
        if (ReferenceEquals(left, right))
            return true;
        if (left is null || right is null)
            return false;
        return left.Equals(right);
    }

    public static bool operator !=(Matrix left, Matrix right)
    {
        return !(left == right);
    }

    public override int GetHashCode()
    {
        unchecked // 允许整数溢出而不抛出异常
        {
            int hash = 17;
            hash = hash * 23 + Rows.GetHashCode();
            hash = hash * 23 + Columns.GetHashCode();

            // 只计算部分元素以提高性能，同时保持合理的哈希分布
            int elementsToHash = Math.Min(10, Rows * Columns);
            for (int i = 0; i < elementsToHash; i++)
            {
                int row = i / Columns;
                int col = i % Columns;
                hash = hash * 23 + this[row, col].GetHashCode();
            }

            return hash;
        }
    }

    #endregion

    #region 矩阵操作

    // 转置矩阵
    public Matrix Transpose()
    {
        var result = new Matrix(Columns, Rows);
        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Columns; j++)
            {
                result[j, i] = this[i, j];
            }
        }
        return result;
    }

    // 余子式矩阵
    public Matrix MinorMatrix(int rowToRemove, int colToRemove)
    {
        if (Rows <= 1 || Columns <= 1)
            throw new InvalidOperationException("矩阵太小，无法删除行列");

        var result = new Matrix(Rows - 1, Columns - 1);

        for (int i = 0; i < Rows; i++)
        {
            if (i == rowToRemove) continue;

            for (int j = 0; j < Columns; j++)
            {
                if (j == colToRemove) continue;

                result[i<rowToRemove ? i : i-1, j < colToRemove ? j : j-1] = this[i, j];
            }
        }

        return result;
    }

    // 矩阵的行列式
    public virtual double Determinant()
    {
        if (!IsSquare)
            throw new InvalidOperationException("只有方阵才能计算行列式");

        // 1x1矩阵
        if (Rows == 1) return this[0, 0];

        // 2x2矩阵
        if (Rows == 2)
            return this[0, 0] * this[1, 1] - this[0, 1] * this[1, 0];

        // 3x3及以上矩阵使用拉普拉斯展开按第i=0行展开
        double det = 0;
        for (int j = 0; j < Columns; j++)
        {
            det += this[0, j] * Cofactor(0, j);
        }

        return det;
    }

    // 代数余子式
    public double Cofactor(int row, int col)
    {
        return Math.Pow(-1, row + col) * MinorMatrix(row, col).Determinant();
    }

    // 伴随矩阵
    public Matrix Adjugate()
    {
        if (!IsSquare)
            throw new InvalidOperationException("只有方阵才能计算伴随矩阵");

        Matrix adjugate = new Matrix(Rows, Columns);

        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Columns; j++)
            {
                adjugate[j, i] = Cofactor(i, j);
            }
        }

        return adjugate;
    }

    // 逆矩阵
    public virtual Matrix Inverse()
    {
        if (!IsSquare)
            throw new InvalidOperationException("只有方阵才能求逆");

        double det = Determinant();
        if (Math.Abs(det) < double.Epsilon)
            throw new InvalidOperationException("矩阵不可逆，行列式为零");

        return Adjugate() * (1.0 / det);
    }

    #endregion

    #region 辅助方法

    public double[,] ToArray()
    {
        return (double[,])data.Clone();
    }

    public override string ToString()
    {
        var sb = new System.Text.StringBuilder("\n");
        for (int i = 0; i < Rows; i++)
        {
            sb.Append("[ ");
            for (int j = 0; j < Columns; j++)
            {
                sb.Append(data[i, j].ToString("F2")).Append(" ");
            }
            sb.Append("]\n");
        }
        return sb.ToString();
    }

    // 按行优先
    public double[] RowFlatten()
    {
        int rows = this.Rows;
        int cols = this.Columns;
        double[] flattened = new double[rows * cols];
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                flattened[i * cols + j] = this[i, j];
            }
        }
        return flattened;
    }

    // 按列优先
    public double[] ColumnFlatten()
    {
        int rows = this.Rows;
        int cols = this.Columns;
        double[] flattened = new double[rows * cols];

        for (int j = 0; j < cols; j++)
        {
            for (int i = 0; i < rows; i++)
            {
                flattened[j * rows + i] = this[i, j];
            }
        }

        return flattened;
    }

    #endregion
}

/// <summary>
/// 2x2矩阵的优化实现
/// </summary>
public class Matrix2x2 : Matrix
{
    // 提供命名属性访问
    public double M11 { get => this[0, 0]; set => this[0, 0] = value; }
    public double M12 { get => this[0, 1]; set => this[0, 1] = value; }
    public double M21 { get => this[1, 0]; set => this[1, 0] = value; }
    public double M22 { get => this[1, 1]; set => this[1, 1] = value; }

    public Matrix2x2() : base(2, 2) { }

    public Matrix2x2(double m11, double m12, double m21, double m22) : this()
    {
        M11 = m11;
        M12 = m12;
        M21 = m21;
        M22 = m22;
    }

    public static Matrix2x2 Identity2x2 => new Matrix2x2(1, 0, 0, 1);
    public static Matrix2x2 Zero2x2 => new Matrix2x2(0, 0, 0, 0);

    public override double Determinant()
    {
        return M11 * M22 - M12 * M21;
    }

    public override Matrix2x2 Inverse()
    {
        double det = Determinant();
        if (Math.Abs(det) < double.Epsilon)
            throw new InvalidOperationException("矩阵不可逆，行列式为零");

        double invDet = 1.0 / det;
        return new Matrix2x2(
            M22 * invDet, -M12 * invDet,
            -M21 * invDet, M11 * invDet);
    }
}

/// <summary>
/// 3x3矩阵的优化实现
/// </summary>
public class Matrix3x3 : Matrix
{
    // 提供命名属性访问
    public double M11 { get => this[0, 0]; set => this[0, 0] = value; }
    public double M12 { get => this[0, 1]; set => this[0, 1] = value; }
    public double M13 { get => this[0, 2]; set => this[0, 2] = value; }
    public double M21 { get => this[1, 0]; set => this[1, 0] = value; }
    public double M22 { get => this[1, 1]; set => this[1, 1] = value; }
    public double M23 { get => this[1, 2]; set => this[1, 2] = value; }
    public double M31 { get => this[2, 0]; set => this[2, 0] = value; }
    public double M32 { get => this[2, 1]; set => this[2, 1] = value; }
    public double M33 { get => this[2, 2]; set => this[2, 2] = value; }

    public Matrix3x3() : base(3, 3) { }

    public Matrix3x3(
        double m11, double m12, double m13,
        double m21, double m22, double m23,
        double m31, double m32, double m33) : this()
    {
        M11 = m11; M12 = m12; M13 = m13;
        M21 = m21; M22 = m22; M23 = m23;
        M31 = m31; M32 = m32; M33 = m33;
    }

    public static Matrix3x3 Identity3x3 => new Matrix3x3(
        1, 0, 0,
        0, 1, 0,
        0, 0, 1);

    public static Matrix3x3 Zero3x3 => new Matrix3x3(
        0, 0, 0,
        0, 0, 0,
        0, 0, 0);

    public override double Determinant()
    {
        return M11 * (M22 * M33 - M23 * M32)
             - M12 * (M21 * M33 - M23 * M31)
             + M13 * (M21 * M32 - M22 * M31);
    }
}
